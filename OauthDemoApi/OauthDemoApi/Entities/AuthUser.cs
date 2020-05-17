using Microsoft.AspNetCore.Identity;

namespace OauthDemoApi.Entities
{
    public class AuthUser : IdentityUser<string>
    {
        public string PictureUrl { get; set; }

        public string Name { get; set; }
    }
}
