using System;
using System.Collections.Generic;
using System.Linq;
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