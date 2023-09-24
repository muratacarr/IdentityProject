using System.ComponentModel.DataAnnotations;

namespace IdentityProject.ViewModels
{
    public class SignInViewModel
    {
        [EmailAddress(ErrorMessage ="Email alanı boş bırakılamaz")]
        [Required(ErrorMessage ="Email alanı boş bırakılamaz")]
        public string? Email { get; set; }

        [Required(ErrorMessage ="Parola kısmı boş bırakılamaz")]
        public string? Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
