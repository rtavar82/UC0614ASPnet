using System;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace UC0614_ExFinal
{
    public static class EmailService
    {
        public static void EnviarAtivacao(string destino, string nome, string token)
        {
            string utilizador =
                Environment.GetEnvironmentVariable("UC0614_SMTP_USER");

            string password =
                Environment.GetEnvironmentVariable("UC0614_SMTP_PASSWORD");

            if (String.IsNullOrWhiteSpace(utilizador) ||
                String.IsNullOrWhiteSpace(password))
            {
                throw new SmtpException(
                    "As credenciais SMTP não estão configuradas.");
            }

            // Obtém automaticamente o endereço da aplicação.
            HttpRequest request = HttpContext.Current.Request;

            string baseUrl =
                request.Url.GetLeftPart(UriPartial.Authority) +
                VirtualPathUtility.ToAbsolute("~/");

            string url =
                baseUrl +
                "ActivateAccount.aspx?token=" +
                HttpUtility.UrlEncode(token);

            using (MailMessage email = new MailMessage())
            using (SmtpClient servidor =
                   new SmtpClient("smtp.gmail.com", 587))
            {
                email.From =
                    new MailAddress(
                        utilizador,
                        "UC0614 Gestão de Contas");

                email.To.Add(destino);

                email.Subject = "Ativação de conta";

                email.IsBodyHtml = true;

                email.Body =
                    "Olá " + HttpUtility.HtmlEncode(nome) +
                    ",<br><br>" +
                    "Ative a sua conta através deste link:<br>" +
                    "<a href='" + url + "'>Ativar conta</a>";

                servidor.EnableSsl = true;
                servidor.UseDefaultCredentials = false;

                servidor.Credentials =
                    new NetworkCredential(
                        utilizador,
                        password);

                servidor.Send(email);
            }
        }

        public static void EnviarRecuperacao(string destino, string token)
        {
            string utilizador =
                Environment.GetEnvironmentVariable("UC0614_SMTP_USER");

            string password =
                Environment.GetEnvironmentVariable("UC0614_SMTP_PASSWORD");

            if (String.IsNullOrWhiteSpace(utilizador) ||
                String.IsNullOrWhiteSpace(password))
            {
                throw new SmtpException(
                    "As credenciais SMTP não estão configuradas.");
            }

            HttpRequest request = HttpContext.Current.Request;

            string baseUrl =
                request.Url.GetLeftPart(UriPartial.Authority) +
                VirtualPathUtility.ToAbsolute("~/");

            string url =
                baseUrl +
                "ResetPassword.aspx?token=" +
                HttpUtility.UrlEncode(token);

            using (MailMessage email = new MailMessage())
            using (SmtpClient servidor =
                   new SmtpClient("smtp.gmail.com", 587))
            {
                email.From =
                    new MailAddress(
                        utilizador,
                        "UC0614 Gestão de Contas");

                email.To.Add(destino);

                email.Subject =
                    "Recuperação de palavra-passe";

                email.IsBodyHtml = true;

                email.Body =
                    "Foi pedida a recuperação da palavra-passe da sua conta." +
                    "<br><br>" +
                    "Clique no link abaixo para definir uma nova palavra-passe:" +
                    "<br>" +
                    "<a href='" + url + "'>Recuperar palavra-passe</a>" +
                    "<br><br>" +
                    "Este link é válido durante 30 minutos.";

                servidor.EnableSsl = true;

                servidor.UseDefaultCredentials = false;

                servidor.Credentials =
                    new NetworkCredential(
                        utilizador,
                        password);

                servidor.Send(email);
            }
        }

    }
}