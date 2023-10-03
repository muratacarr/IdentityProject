using System.ComponentModel.DataAnnotations;

namespace IdentityProject.Areas.Admin.Models
{
    public class RoleUpdateViewModel
    {
        public string Id { get; set; } = null!;
        [Required(ErrorMessage ="Role isim alanı boş bırakılamaz")]
        public string Name { get; set; } = null!;
    }
}
