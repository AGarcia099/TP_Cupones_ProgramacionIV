using CuponesApiTp.Interfaces;
using System.Net;
using System.Net.Mail;

namespace CuponesApiTp.Services
{
    public class SendEmailService : ISendEmailService
    {
        public async Task EnviarEmailCliente(string emailCliente, string nroCupon, string Subject, string messageBody)
        {
            string emailDesde = "programacioniv.agus@gmail.com";
            string emailClave = "dscq ndky eive xpku";
            string servicioGoogle = "smtp.gmail.com";

            try
            {
                SmtpClient smtpClient = new SmtpClient(servicioGoogle);
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential(emailDesde, emailClave);
                smtpClient.EnableSsl = true;

                MailMessage message = new MailMessage();
                message.From = new MailAddress(emailDesde, "ProgramacionIV");
                message.To.Add(emailCliente);
                message.Subject = Subject;
                message.Body = messageBody;

                await smtpClient.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
