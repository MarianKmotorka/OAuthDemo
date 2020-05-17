using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OauthDemoApi.Entities;

namespace OauthDemoApi.Persistance
{
    public class Db : IdentityDbContext<AuthUser, AuthRole, string>
    {
        public Db(DbContextOptions<Db> options) : base(options)
        {
        }
    }
}
