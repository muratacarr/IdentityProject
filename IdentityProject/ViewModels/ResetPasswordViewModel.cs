using System.ComponentModel.DataAnnotations;

namespace IdentityProject.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Şifre Alanı boş bırakılamaz.")]
        public string? Password { get; set; }
        [Compare(nameof(Password), ErrorMessage = "Şifre aynı değildir.")]
        [Required(ErrorMessage = "Şifre Tekrar Alanı boş bırakılamaz.")]
        public string? PasswordConfirm { get; set; }
    }
}
