using IdentityProject.Context;
using IdentityProject.Entities.Context;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IdentityProject.Extensions
{
    public static class StartupExtensions
    {
        public static void AddIdentityWithExt(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, AppRole>(opt =>
            {
                opt.User.RequireUniqueEmail = true;
                opt.User.AllowedUserNameCharacters = "abcdefghijklmnoprstuvwxyz1234567890_-.";

                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = false;
                opt.Password.RequiredLength = 6;


            }).AddEntityFrameworkStores<AppDbContext>();
        }
    }
}
