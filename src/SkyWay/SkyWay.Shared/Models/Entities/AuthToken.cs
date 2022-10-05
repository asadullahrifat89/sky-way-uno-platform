using System;

namespace SkyWay
{
    public class AuthToken
    {
        public string AccessToken { get; set; } = string.Empty;

        public DateTime ExpiresOn { get; set; }
    }
}