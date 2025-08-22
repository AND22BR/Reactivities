using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Email
{
    public class EmailSender(IMailgun mailgun) : IEmailSender<User>
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

        public Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
        {
            throw new NotImplementedException();
        }

        public Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
        {
            throw new NotImplementedException();
        }
    }
}