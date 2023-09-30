using System.ComponentModel.DataAnnotations;

namespace IdentityProject.ViewModels
{
    public class ForgetPasswordViewModel
    {
        [EmailAddress(ErrorMessage = "Email formatına uygun bir mail giriniz")]
        [Required(ErrorMessage = "Email alanı boş bırakılamaz")]
        public string? Email { get; set; }
    }
}
