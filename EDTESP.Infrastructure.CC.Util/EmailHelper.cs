using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace EDTESP.Infrastructure.CC.Util
{
    public static class EmailHelper
    {
        private static SmtpClient MountClient(int opcao = 0)
        {
            var resultGetDataSmtp = GetDataSmtp(opcao);

            var smtpHost = resultGetDataSmtp["smtpHost"].ToString();
            var smtpPort = Convert.ToInt32(resultGetDataSmtp["smtpPort"]);
            var userSmtp = resultGetDataSmtp["userSmtp"].ToString();
            var passwordSmtp = resultGetDataSmtp["passwordSmtp"].ToString();
            var enabledSsl = Convert.ToBoolean(resultGetDataSmtp["enabledSsl"]);

            var smtp = new SmtpClient(smtpHost, smtpPort);
            var creds = new NetworkCredential(userSmtp, passwordSmtp);
            smtp.EnableSsl = enabledSsl;
            smtp.Credentials = creds;
            smtp.UseDefaultCredentials = false;

            return smtp;
        }

        private static MailMessage MountMessage(Dictionary<string, string> to, string subject, string body, Dictionary<string, string> bcc = null, string fromEmail = "")
        {
            MailAddress from;

            if (string.IsNullOrEmpty(fromEmail))
                from = new MailAddress(EdtespConfig.SmtpUser, EdtespConfig.SmtpSender);
            else
                from = new MailAddress(fromEmail, RetornarNomePorEmail(fromEmail));

            var msg = new MailMessage
            {
                IsBodyHtml = true,
                From = from,
                Subject = subject,
                Body = body,
            };

            to.ToList().ForEach(x => msg.To.Add(new MailAddress(x.Key, x.Value)));

            if (bcc != null && bcc.ToList().Any())
                bcc.ToList().ForEach(x => msg.Bcc.Add(new MailAddress(x.Key, x.Value)));

            return msg;
        }

        public static void SendEmail(Dictionary<string, string> to, string subject, string body, string[] attachments, Dictionary<string, string> bcc = null, int opcao = 0)
        {
            var smtp = MountClient(opcao);

            var dataEmail = GetDataSmtp(opcao);

            var msg = MountMessage(to, subject, body, bcc, dataEmail["userSmtp"].ToString());

            if (attachments.Any())
            {
                foreach (var attach in attachments)
                {
                    var att = new Attachment(attach);
                    msg.Attachments.Add(att);
                }
            }

            smtp.Send(msg);
        }

        public static void SendEmail(Dictionary<string, string> to, string subject, string body, Dictionary<string, byte[]> attachments, Dictionary<string, string> bcc = null, string from = "", int opcao = 0)
        {
            var smtp = MountClient(opcao);

            var dataEmail = GetDataSmtp(opcao);

            var msg = MountMessage(to, subject, body, bcc, dataEmail["userSmtp"].ToString());

            if (attachments.Any())
            {
                foreach (var attach in attachments)
                {
                    var stream = new MemoryStream(attach.Value);
                    var att = new Attachment(stream, attach.Key);
                    msg.Attachments.Add(att);
                }
            }

            smtp.Send(msg);
        }

        private static Dictionary<string, object> GetDataSmtp(int opcao)
        {
            var result = new Dictionary<string, object>();

            if (opcao == 0 || opcao == 1)
            {
                result.Add("smtpHost", EdtespConfig.SmtpHost);
                result.Add("smtpPort", EdtespConfig.SmtpPort);
                result.Add("userSmtp", EdtespConfig.SmtpUser);
                result.Add("passwordSmtp", EdtespConfig.SmtpPass);
                result.Add("enabledSsl", EdtespConfig.SmtpSsl);
            }
            else
            {
                var dataString = "";

                switch (opcao)
                {
                    case 2:
                        dataString = EdtespConfig.ClanespEmail;
                        break;
                    case 3:
                        dataString = EdtespConfig.GuiaFazEmail;
                        break;
                    case 4:
                        dataString = EdtespConfig.EditoraNacionalEmail;
                        break;
                    case 5:
                        dataString = EdtespConfig.ListaFacilEmail;
                        break;
                    default:
                        result.Add("smtpHost", EdtespConfig.SmtpHost);
                        result.Add("smtpPort", EdtespConfig.SmtpPort);
                        result.Add("userSmtp", EdtespConfig.SmtpUser);
                        result.Add("passwordSmtp", EdtespConfig.SmtpPass);
                        result.Add("enabledSsl", EdtespConfig.SmtpSsl);
                        break;
                }

                if (!string.IsNullOrEmpty(dataString))
                {
                    var arrayData = dataString.Split(';');

                    result.Add("smtpHost", arrayData[0]);
                    result.Add("smtpPort", arrayData[1]);
                    result.Add("userSmtp", arrayData[2]);
                    result.Add("passwordSmtp", arrayData[3]);
                    result.Add("enabledSsl", arrayData[4]);
                }
            }

            return result;
        }

        private static string RetornarNomePorEmail(string email, int opcao = 0)
        {
            var resultado = "";

            if (email == "clanespcontrato@gmail.com")
                resultado = "clanespcontrato";
            else if (email == "guiafazcontrato@gmail.com")
                resultado = "guiafacilcontrato";
            else if(email == "editnac@gmail.com")
                resultado = "editoranacional";
            else if(email == "listafacilcontrato@gmail.com")
                resultado = "listafacilcontrato";
            else
                resultado = EdtespConfig.SmtpSender;

            return resultado;
        }
    }
}