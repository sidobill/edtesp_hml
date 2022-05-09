using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Edtesp.Ftp
{
    public static class Ftp
    {
        public static Dictionary<bool, string> UploadSingeFile(string pathFile)
        {
            var url = "ftp://saopaulo.braslink.com";
            var user = "edtespcom";
            var password = "b8LORw8rUj";

            var result = new Dictionary<bool, string>();

            try
            {
                var info = new FileInfo(pathFile);

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url + @"/" + "public_html/painel/assets/img/logos/" + info.Name);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(user, password);
                request.UseBinary = true;
                request.ContentLength = info.Length;

                using (FileStream fs = info.OpenRead())
                {
                    byte[] buffer = new byte[2048];
                    int bytesSent = 0;
                    int bytes = 0;
                    using (Stream stream = request.GetRequestStream())
                    {
                        while (bytesSent < info.Length)
                        {
                            bytes = fs.Read(buffer, 0, buffer.Length);
                            stream.Write(buffer, 0, bytes);
                            bytesSent += bytes;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                result.Add(false, ex.Message);
            }

            return result;
        }
    }
}
