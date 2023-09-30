using IdentityProject.OptionsModels;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace IdentityProject.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailsettings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _emailsettings = options.Value;
        }

        public async Task SendResetPasswordEmail(string resetEmail, string to)
        {
            var smtpClient=new SmtpClient();

            smtpClient.Host = _emailsettings.Host;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials=new NetworkCredential(_emailsettings.Email,_emailsettings.Password);

            var mailMessage=new MailMessage();

            mailMessage.From = new MailAddress(_emailsettings.Email);
            mailMessage.To.Add(to);

            mailMessage.Subject = "MuratApp Şifre Sıfırlama Linki";
            mailMessage.Body = @$"
                                <h3>Şifrenizi sıfrlamak için aşağıdaki linke tıklayınız</h3>
                                <p><a href='{resetEmail}'>Şifre yenileme linki</a></p>";
            mailMessage.IsBodyHtml = true;

            await smtpClient.SendMailAsync(mailMessage);

        }
    }
}
