using MATO.Classes;

namespace MATO.NET.Interfaces
{
    public interface IEmailService
    {
        bool SendMail(AppUser emailTo, int messageId, Requests request);
        bool NewUserMail(AppUser emailTo, string password, string roleName, string callbackUrl);
        bool SendWelcomeEmail(AppUser emailTo, string password, string role, string baseUrl);
    }
}
