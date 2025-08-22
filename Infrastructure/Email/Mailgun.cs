using System;
using RestSharp; // RestSharp v112.1.0
using RestSharp.Authenticators;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Email
{
    public interface IMailgun
    {
        Task<RestResponse> Send(string to, string subject, string body);
    }

    public class Mailgun : IMailgun
    {
        private readonly IConfiguration _configuration;
        private string _apiKey = "";
        public Mailgun(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiKey = _configuration.GetSection("Mailgun:ApiKey").Value;
        }

        public async Task<RestResponse> Send(string to, string subject, string body)
        {
            var options = new RestClientOptions("https://api.mailgun.net")
            {
                Authenticator = new HttpBasicAuthenticator("api", _apiKey)
            };

            var client = new RestClient(options);
            var request = new RestRequest("/v3/notifications.energyadepto.com/messages", Method.Post);
            request.AlwaysMultipartFormData = true;
            request.AddParameter("from", "Energy Adepto <no-reply@notifications.energyadepto.com>");
            request.AddParameter("to", to);
            request.AddParameter("subject", subject);
            request.AddParameter("html", body);
            return await client.ExecuteAsync(request);
        }
    }
}