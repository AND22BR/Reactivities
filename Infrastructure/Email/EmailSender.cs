using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Email
{
    public class EmailSender(IMailgun mailgun, IConfiguration config) : IEmailSender<User>
    {
        public async Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
        {
            var subject = "Confirm your email address";
            var body = $@"
                <p>Hi {user.DisplayName}</p>
                <p>Please confirm your email by clicking the link below</p>
                <p><a href='{confirmationLink}'>Click here to verify email</a></p>
                <p>Thanks</p>
            ";

            await mailgun.Send(email, subject, body);
        }

        public async Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
        {
            var subject = "Reset your password";
            var body = $@"
                <p>Hi {user.DisplayName}</p>
                <p>Please this link to reset your password</p>
                <p><a href='{config["ClientAppUrl"]}/reset-password?email={email}&code={resetCode}'>
                    Click to reset your password
                </a></p>
                <p>If you did not request this, contact support.</p>
            ";

            await mailgun.Send(email, subject, body);
        }

        public Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
        {
            throw new NotImplementedException();
        }
    }
}