using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using API.DTOs;
using Application.UserData.Commands;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Persistence;
using static API.DTOs.MicrosoftInfo;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : BaseApiController
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender<User> _emailSender;
        private readonly IConfiguration _config;

        public AccountController(SignInManager<User> signInManager,
        IEmailSender<User> emailSender,
        IConfiguration config)
        {
            _signInManager = signInManager;
            _emailSender = emailSender;
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost("microsoft-login")]
        public async Task<ActionResult> LoginWithMicrosoft(string code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("Missing authorization code");

            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var content=new FormUrlEncodedContent(new Dictionary<string,string>()
                {
                    {MicrosoftAuthRequest.GetJsonPropName(nameof(MicrosoftAuthRequest.Code)), code },
                    {MicrosoftAuthRequest.GetJsonPropName(nameof(MicrosoftAuthRequest.ClientId)),  _config["Authentication:Microsoft:ClientId"]! },
                    {MicrosoftAuthRequest.GetJsonPropName(nameof(MicrosoftAuthRequest.ClientSecret)),   _config["Authentication:Microsoft:ClientSecret"]! },
                    {MicrosoftAuthRequest.GetJsonPropName(nameof(MicrosoftAuthRequest.RedirectUri)), $"{_config["ClientAppUrl"]}/auth-callback" },
                    {MicrosoftAuthRequest.GetJsonPropName(nameof(MicrosoftAuthRequest.GrantType)), "authorization_code" },
                    {MicrosoftAuthRequest.GetJsonPropName(nameof(MicrosoftAuthRequest.Scope)),"https://graph.microsoft.com/User.Read" }
                });

            //step 1 - exchange code for access token
            var tokenResponse = await httpClient.PostAsync(
                $"https://login.microsoftonline.com/{_config["Authentication:Microsoft:TenantId"]}/oauth2/v2.0/token",
                content
            );

            var response=await tokenResponse.Content.ReadAsStringAsync();

            if (!tokenResponse.IsSuccessStatusCode)
                return BadRequest("Failed to get access token");

            var tokenContent = await tokenResponse.Content.ReadFromJsonAsync<MicrosoftTokenResponse>();

            if (string.IsNullOrEmpty(tokenContent?.AccessToken))
                return BadRequest("Failed to retrieve access token");

            //step 2 - fetch user info from Microsoft
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenContent.AccessToken);
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Reactivities");

            var userResponse = await httpClient.GetAsync("https://graph.microsoft.com/v1.0/me");
            if (!userResponse.IsSuccessStatusCode)
                return BadRequest("Failed to fetch user from Microsoft");

            var user = await userResponse.Content.ReadFromJsonAsync<MicrosoftUser>();
            if (user is null)
                return BadRequest("Failed to load user from Microsoft");

            //step 3 - getting the profile photo
            // var userPofileResponse = await httpClient.GetAsync("https://graph.microsoft.com/v1.0/me/photo/$value");
            // using var userProfilePhoto= await userPofileResponse.Content.ReadAsStreamAsync();
            // userProfilePhoto.
            //user.ImageUrl = userProfilePhoto;

            //step 4 - adding the user
            var existingUser = await _signInManager.UserManager.FindByEmailAsync(user.Email);
            if (existingUser == null)
            {
                existingUser = new User
                {
                    Email = user.Email,
                    UserName = user.Email,
                    DisplayName = user.Name,
                    ImageUrl = user.ImageUrl
                };

                var createdResult = await _signInManager.UserManager.CreateAsync(existingUser);
                if (!createdResult.Succeeded)
                    return BadRequest("Failed to create user");

                var userResult = await _signInManager.UserManager.FindByEmailAsync(user.Email);
                var newUserInfo = new UserData()
                {
                    Id = userResult.Id,
                    Email = userResult.Email,
                    UserName = userResult.UserName,
                    DisplayName = userResult.DisplayName,
                    Bio = userResult.Bio,
                    ImageUrl = userResult.ImageUrl
                };

                try
                {
                    await Mediator.Send(new CreateUserData.Command { UserData = newUserInfo });
                }
                catch (System.Exception ex)
                {
                    await _signInManager.UserManager.DeleteAsync(userResult);
                    return BadRequest("Couldn't create user data. Try again or contact support");
                }

            }

            await _signInManager.SignInAsync(existingUser, false);

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser(RegisterDto registerDto)
        {
            var user = new User()
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                DisplayName = registerDto.DisplayName
            };

            var result = await _signInManager.UserManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return ValidationProblem();
            }
            else
            {
                await SendConfirmationEmailAsync(user, registerDto.Email);
            }

            var userResult = await _signInManager.UserManager.FindByEmailAsync(user.Email);

            var newUserInfo = new UserData()
            {
                Id = userResult.Id,
                Email = userResult.Email,
                UserName = userResult.UserName,
                DisplayName = userResult.DisplayName,
                Bio = userResult.Bio,
                ImageUrl = userResult.ImageUrl
            };

            try
            {
                await Mediator.Send(new CreateUserData.Command { UserData = newUserInfo });
            }
            catch (System.Exception ex)
            {
                await _signInManager.UserManager.DeleteAsync(userResult);
                return BadRequest("Couldn't create user data. Try again or contact support");
            }

            return Ok();

        }

        [AllowAnonymous]
        [HttpGet("resendConfirmEmail")]
        public async Task<ActionResult> ResendConfirmEmail(string? email, string? userId)
        {
            if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(userId))
            {
                return BadRequest("Email or user id must be provided");
            }

            var user = await _signInManager.UserManager.Users.FirstOrDefaultAsync(x => x.Email == email || x.Id == userId);

            if (user is null) return BadRequest("User not found");

            await SendConfirmationEmailAsync(user, user.Email);
            return Ok();
        }

        private async Task SendConfirmationEmailAsync(User user, string email)
        {
            var code = await _signInManager.UserManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var confirmEmailUrl = $"{_config["ClientAppUrl"]}/confirm-email?userId={user.Id}&code={code}";
            await _emailSender.SendConfirmationLinkAsync(user, user.Email, confirmEmailUrl);
        }

        [AllowAnonymous]//Set anonymous to check validation
        [HttpGet("user-info")]
        public async Task<ActionResult> GetUserInfo()
        {
            if (User.Identity?.IsAuthenticated == false) return NoContent();

            var user = await _signInManager.UserManager.GetUserAsync(User);

            if (user == null) return Unauthorized();

            return Ok(new
            {
                user.DisplayName,
                user.Email,
                user.ImageUrl,
                user.Id
            });

        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return NoContent();
        }

        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var user = await _signInManager.UserManager.GetUserAsync(User);

            if (user is null) return Unauthorized();

            var result = await _signInManager.UserManager.ChangePasswordAsync(user,
            changePasswordDto.CurrentPassword,
            changePasswordDto.NewPassword);

            if (result.Succeeded) return Ok();

            return BadRequest(result.Errors.First().Description);
        }
    }
}