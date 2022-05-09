using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Text;
using EDTESP.Web.Helpers.JsonModels;
using Newtonsoft.Json;
using RestSharp;

namespace EDTESP.Web.Helpers
{
    public class IntegracaoHelper
    {
        public static string Baseurl => ConfigurationManager.AppSettings["IntegradorUrl"];

        private static RestClient GetClient()
        {
            var req = new RestClient(Baseurl);
            req.Encoding = Encoding.UTF8;

            return req;

        }

        private static RestRequest GetRequest(string resource, Method mth)
        {
            var cli = new RestRequest(mth);
            cli.Resource = resource;

            return cli;

        }

        public static IEnumerable<Classificados> ObterClassificados()
        {
            var cli = GetClient();

            var req = GetRequest("classificados", Method.GET);
            var res = cli.Execute(req);

            if(res.StatusCode != HttpStatusCode.OK)
                throw new Exception($"Erro ao obter dados do site {res.Content}");

            var lst = JsonConvert.DeserializeObject<IEnumerable<Classificados>>(res.Content);
            return lst;
        }

        public static Classificados ObterClassificado(int id)
        {
            var cli = GetClient();

            var req = GetRequest($"classificados/{id}", Method.GET);
            var res = cli.Execute(req);

            if (res.StatusCode != HttpStatusCode.OK)
                throw new Exception($"Erro ao obter dados do site {res.Content}");

            var lst = JsonConvert.DeserializeObject<Classificados>(res.Content);
            return lst;
        }

        public static Clientes ObterCliente(int id)
        {
            var cli = GetClient();

            var req = GetRequest($"clientes/{id}", Method.GET);
            var res = cli.Execute(req);

            if (res.StatusCode != HttpStatusCode.OK)
                throw new Exception($"Erro ao obter dados do site {res.Content}");

            var lst = JsonConvert.DeserializeObject<Clientes>(res.Content);
            return lst;
        }

        public static void InserirCliente(Clientes obj)
        {
            var cli = GetClient();

            var req = GetRequest($"clientes", Method.POST);
            req.AddJsonBody(obj);

            var res = cli.Execute(req);

            if (res.StatusCode != HttpStatusCode.OK)
                throw new Exception($"Erro ao enviar dados ao site {res.Content}");
        }

        public static void AtualizarCliente(int id, Clientes obj)
        {
            var cli = GetClient();

            var req = GetRequest($"clientes/{id}", Method.PUT);
            req.AddJsonBody(obj);

            var res = cli.Execute(req);

            if (res.StatusCode != HttpStatusCode.OK)
                throw new Exception($"Erro ao enviar dados ao site {res.Content}");
        }

        public static void UploadFile(string caminho, string filename, byte[] data)
        {
            var cli = GetClient();
            var req = GetRequest("ftp", Method.POST);
            req.AddFileBytes("file", data, filename);
            req.AddParameter("ftppath", caminho);

            var res = cli.Execute(req);

            if (res.StatusCode != HttpStatusCode.OK)
                throw new Exception($"Erro ao enviar dados ao site por ftp {res.Content}");
        }
    }
}