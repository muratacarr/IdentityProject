namespace IdentityProject.Services
{
    public interface IEmailService
    {
        Task SendResetPasswordEmail(string resetEmail,string to);
    }
}
