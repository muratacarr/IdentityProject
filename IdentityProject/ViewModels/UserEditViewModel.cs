using IdentityProject.Entities;
using System.ComponentModel.DataAnnotations;

namespace IdentityProject.ViewModels
{
    public class UserEditViewModel
    {
        [Required(ErrorMessage = "Kullanıcı Ad Alanı boş bırakılamaz.")]
        public string Username { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Email formatı yanlıştır.")]
        [Required(ErrorMessage = "Email Alanı boş bırakılamaz.")]
        public string? Email { get; set; } = null!;

        [Required(ErrorMessage = "Phone Alanı boş bırakılamaz.")]
        public string? Phone { get; set; } = null!;

        public DateTime? BirthDate { get; set; }
        public string? City { get; set; }
        public IFormFile? Picture { get; set; }
        public Gender? ModelGender { get; set; }
    }

}
