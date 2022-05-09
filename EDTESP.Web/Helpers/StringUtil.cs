using System;

namespace EDTESP.Web.Helpers
{
    public class StringUtil
    {
        public static string RandomString(int len = 64)
        {
            var caracs = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var str = string.Empty;

            var rnd = new Random();
            while (str.Length < len)
            {
                var i = rnd.Next(0,len-1);
                str += caracs[i];
            }

            return str;
        }
    }
}