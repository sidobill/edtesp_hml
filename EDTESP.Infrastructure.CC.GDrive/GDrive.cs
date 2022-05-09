using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using EDTESP.Infrastructure.CC.Util;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using File = Google.Apis.Drive.v3.Data.File;

namespace EDTESP.Infrastructure.CC.GDrive
{
    public class GDrive
    {
        private static string[] _scopes = new[] {
            DriveService.Scope.DriveAppdata
        };

        private static DriveService CriarConexao()
        {
            UserCredential credenciais;
            using (var stream = new System.IO.FileStream(Path.Combine(EdtespConfig.BinDir, "edtesp.json"), System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {

                var diretorioCredenciais = System.IO.Path.Combine(EdtespConfig.BinDir, "token.json");

                credenciais = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    _scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(diretorioCredenciais, true)).Result;
            }

            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credenciais
            });
        }

        public static void UploadFile(Stream data, string name, string mimeType, string id)
        {
            var service = CriarConexao();

            var md = new File
            {
                Name = name,
                Parents = new List<string>()
                {
                    "appDataFolder"
                }
            };

            FilesResource.CreateMediaUpload request;
            request = service.Files.Create(md, data, mimeType);
            request.Fields = id;
            request.Upload();
            
        }
    }
}