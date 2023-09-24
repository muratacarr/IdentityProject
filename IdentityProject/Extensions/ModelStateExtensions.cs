using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IdentityProject.Extensions
{
    /// <summary>
    /// Bu sınıf model state e hataları eklemek için kullanılır
    /// </summary>
    public static class ModelStateExtensions
    {
        public static void AddModelErrorList(this ModelStateDictionary modelState,List<string> errors)
        {
            errors.ForEach(x =>
            {
                modelState.AddModelError(string.Empty, x);
            });
        }
    }
}
