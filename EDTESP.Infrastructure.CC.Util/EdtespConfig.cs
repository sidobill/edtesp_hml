using System;
using System.Configuration;
using System.IO;

namespace EDTESP.Infrastructure.CC.Util
{
    public class EdtespConfig
    {
        public static string AppDataFolder => ConfigurationManager.AppSettings["AppDataFolder"];

        public static string GNetChave => ConfigurationManager.AppSettings["GNetChave"];

        public static string GNetSecret => ConfigurationManager.AppSettings["GNetSecret"];

        public static bool GNetSandbox => Convert.ToBoolean(ConfigurationManager.AppSettings["GNetSandbox"]);

        public static string BinDir => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");

        public static string SmtpHost => ConfigurationManager.AppSettings["SmtpHost"];

        public static int SmtpPort => Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);

        public static bool SmtpSsl => Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpSSL"]);

        public static string SmtpUser => ConfigurationManager.AppSettings["SmtpUser"];

        public static string SmtpPass => ConfigurationManager.AppSettings["SmtpPass"];

        public static string SmtpSender => ConfigurationManager.AppSettings["SmtpSender"];

        public static string ApiUser => "edtesp_api";

        public static string ApiPass => "3dt3sp!2018";

        public static string ClanespEmail => ConfigurationManager.AppSettings["ClanespEmail"];

        public static string GuiaFazEmail => ConfigurationManager.AppSettings["GuiaFazEmail"];

        public static string ListaFacilEmail => ConfigurationManager.AppSettings["ListaFacil"];

        public static string EditoraNacionalEmail => ConfigurationManager.AppSettings["EditoraNacionalEmail"];
    }
}