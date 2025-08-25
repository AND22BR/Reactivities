using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace API.DTOs
{
    public class MicrosoftInfo
    {
        public class MicrosoftAuthRequest
        {
            [JsonPropertyName("code")]
            public required string Code { get; set; }
            [JsonPropertyName("client_id")]
            public required string ClientId { get; set; }
            [JsonPropertyName("client_secret")]
            public required string ClientSecret { get; set; }
            [JsonPropertyName("redirect_uri")]
            public required string RedirectUri { get; set; }
            [JsonPropertyName("grant_type")]
            public string GrantType { get; set; } = "authorization_code";
            [JsonPropertyName("scope")]
            public string Scope { get; set; } = "https%3A%2F%2Fgraph.microsoft.com%2Fmail.read";


            public static string GetJsonPropName(string propertyName)
            {
                return typeof(MicrosoftAuthRequest)
                    .GetProperty(propertyName)
                    .GetCustomAttribute<JsonPropertyNameAttribute>()
                    .Name;
            }
        }

        public class MicrosoftTokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }
        }

        public class MicrosoftUser
        {
            [JsonPropertyName("mail")]
            public string Email { get; set; }
            [JsonPropertyName("displayName")]
            public string Name { get; set; }
            public string? ImageUrl { get; set; }
        }

        public class MicrosoftProfileImage
        {
            public string? ImageUrl { get; set; }
        }
    }
}