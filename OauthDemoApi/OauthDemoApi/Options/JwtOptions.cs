using System;

namespace OauthDemoApi.Options
{
    public class JwtOptions
    {
        public TimeSpan TokenLifeTime { get; set; }

        public string Secret { get; set; }

        public string Issuer { get; set; }
    }
}
