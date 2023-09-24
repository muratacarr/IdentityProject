using System.ComponentModel.DataAnnotations;

namespace IdentityProject.ViewModels
{
    public class SignUpViewModel
    {
        [Required(ErrorMessage ="Kullanıcı Ad Alanı boş bırakılamaz.")]
        public string? Username { get; set; }
        [EmailAddress(ErrorMessage ="Email formatı yanlıştır.")]
        [Required(ErrorMessage = "Email Alanı boş bırakılamaz.")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Phone Alanı boş bırakılamaz.")]
        public string? Phone { get; set; }
        [Required(ErrorMessage = "Şifre Alanı boş bırakılamaz.")]
        public string? Password { get; set; }
        [Compare(nameof(Password),ErrorMessage ="Şifre aynı değildir.")]
        [Required(ErrorMessage = "Şifre Tekrar Alanı boş bırakılamaz.")]
        public string? PasswordConfirm { get; set; }
    }
}
