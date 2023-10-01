using System.ComponentModel.DataAnnotations;

namespace IdentityProject.ViewModels
{
    public class PasswordChangeViewModel
    {
        [Required(ErrorMessage = "Şifre Alanı boş bırakılamaz.")]
        public string? PasswordOld { get; set; }
        [Required(ErrorMessage = "Şifre Alanı boş bırakılamaz.")]
        public string? PasswordNew { get; set; }
        [Compare(nameof(PasswordNew), ErrorMessage = "Şifre aynı değildir.")]
        [Required(ErrorMessage = "Şifre Tekrar Alanı boş bırakılamaz.")]
        public string? PasswordNewConfirm { get; set; }
    }
}
