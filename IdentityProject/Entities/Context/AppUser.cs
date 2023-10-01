using Microsoft.AspNetCore.Identity;

namespace IdentityProject.Entities.Context
{
    public class AppUser : IdentityUser
    {
        public string? City { get; set; }
        public string? Picture { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
