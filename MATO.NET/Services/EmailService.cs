using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Policy;
using System.Web;
using MATO.Classes;
using MATO.DataModel;
using MATO.NET.Interfaces;

namespace MATO.NET.Services
{
    public class EmailService : IEmailService
    {
        private readonly MATOContext context;

        public EmailService(MATOContext _context)
        {
            context = _context;
        }

        public bool SendMail(AppUser emailTo, int messageId, Requests request)
        {
            var messageToSend = context.EmailTemplates.FirstOrDefault(m => m.Id == messageId);
            string messageToSendNew = messageToSend.Html;

            string baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            if (messageToSendNew != null)
            {
                messageToSendNew = messageToSendNew.Replace("@@MailTo", emailTo.FullName);
                messageToSendNew = messageToSendNew.Replace("@@Request", request.EventOne.EventName);
                messageToSendNew = messageToSendNew.Replace("@@ReqLink", "<a href=\"" + baseUrl + "/Requests/Request/" + request.Id + "\">View Request</a>");
                messageToSendNew = messageToSendNew.Replace("@@ManagerDecisionLink", "<a href=\"" + baseUrl + "/Decisions/Decision/" + request.Id + "\">View</a>");
                messageToSendNew = messageToSendNew.Replace("@@AdminDecisionLink", "<a href=\"" + baseUrl + "/Decisions/AdminDecision/" + request.Id + "\">View</a>");
                messageToSendNew = messageToSendNew.Replace("@@AuthorDecisionLink", "<a href=\"" + baseUrl + "/Decisions/AuthorDecision/" + request.Id + "\">View</a>");
                messageToSendNew = messageToSendNew.Replace("@@SalesRep", request.SubmitBy.FullName);
                messageToSendNew = messageToSendNew.Replace("@@Author", request.RequestedAuthor.FullName);
                messageToSendNew = messageToSendNew.Replace("@@City", request.EventOne.EventCity);
                messageToSendNew = messageToSendNew.Replace("@@Country", "?");
                messageToSendNew = messageToSendNew.Replace("@@EventDate", request.EventOne.EventDate.ToString("dd MMM yyyy"));
                messageToSendNew = messageToSendNew.Replace("@@TentativeReason", request.TentativeReason);
                
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress("MATO-noreply@macmillaneducation.com")
                };

                SmtpClient smtp = new SmtpClient
                {
                    Port = 25,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                  //  Host = "relay.springernature.com"
                    Host = "localhost"

                };

                //recipient address
                mail.To.Add(new MailAddress(emailTo.Email));

                //Formatted mail body
                mail.IsBodyHtml = true;

                mail.Subject = "MATO Request Update!";
                mail.Body = messageToSendNew;
                smtp.Send(mail);

                return true;
            }

            return false;

        }

        public bool NewUserMail(AppUser emailTo, string password, string role, string callbackUrl)
        {
            var messageToSend = context.EmailTemplates.FirstOrDefault(m => m.Id == 15);

            if (messageToSend != null)
            {
                messageToSend.Html = messageToSend.Html.Replace("@@MailTo", emailTo.FullName);
                messageToSend.Html = messageToSend.Html.Replace("@@Password", "Please reset your password by clicking <a href =\"" + callbackUrl + "\">here</a>");
                messageToSend.Html = messageToSend.Html.Replace("@@Username", emailTo.Email);
                messageToSend.Html = messageToSend.Html.Replace("@@Role", role);

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress("MATO-noreply@macmillaneducation.com")
                };

                SmtpClient smtp = new SmtpClient
                {
                    Port = 25,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Host = "relay.springernature.com"
                };

                //recipient address
                mail.To.Add(new MailAddress(emailTo.Email));

                //Formatted mail body
                mail.IsBodyHtml = true;


                mail.Subject = "MATO Account Creation";
                mail.Body = messageToSend.Html;
                smtp.Send(mail);

                return true;
            }

            return false;
        }

        public bool SendWelcomeEmail(AppUser emailTo, string password, string role, string baseUrl)
        {
            var messageToSend = context.EmailTemplates.FirstOrDefault(m => m.Id == 15);

            if (messageToSend != null)
            {
                messageToSend.Html = messageToSend.Html.Replace("@@MailTo", emailTo.FullName);
                messageToSend.Html = messageToSend.Html.Replace("@@Password",
                    "Please reset your password by requesting a password reset <a href =\"" + baseUrl + "Account/ForgotPassword\">here</a>");
                messageToSend.Html = messageToSend.Html.Replace("@@Username", emailTo.Email);
                messageToSend.Html = messageToSend.Html.Replace("@@Role", role);

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress("MATO-noreply@macmillaneducation.com")
                };

                SmtpClient smtp = new SmtpClient
                {
                    Port = 25,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Host = "relay.springernature.com"
                };

                //recipient address
                mail.To.Add(new MailAddress(emailTo.Email));

                //Formatted mail body
                mail.IsBodyHtml = true;


                mail.Subject = "MATO Account Creation";
                mail.Body = messageToSend.Html;
                smtp.Send(mail);

                return true;
            }

            return false;
        }
    }
}
