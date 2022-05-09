using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using RestSharp;
using RestSharp.Authenticators;

namespace EDTESP.GeradorArquivos
{
    public partial class EdtespService : ServiceBase
    {
        private readonly Timer _tmrGerBol;
        private readonly Timer _tmrEnvEml;

        private EventLog _log;
        
        public EdtespService()
        {
            InitializeComponent();            
            SetupLogging();

            _tmrEnvEml = new Timer();
            _tmrGerBol = new Timer();

            _tmrGerBol.Interval = (30 * 1000); //30s
            _tmrEnvEml.Interval = (30 * 1000); //30s

            _tmrGerBol.Elapsed += TmrGerBol_Elapsed;
            _tmrEnvEml.Elapsed += TmrEnvEml_Elapsed;
        }

        private void TmrEnvEml_Elapsed(object sender, ElapsedEventArgs e)
        {
            EnviarPorEmail();
        }

        private void TmrGerBol_Elapsed(object sender, ElapsedEventArgs e)
        {
            GerarBoletos();
        }

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry("Serviço iniciado");
            _tmrGerBol.Start();
            _tmrEnvEml.Start();
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry("Serviço parado");
            _tmrGerBol.Stop();
            _tmrEnvEml.Stop();
        }

        private RestClient GerarClient()
        {
            var url = ConfigurationManager.AppSettings["UrlApi"];
            var usr = ConfigurationManager.AppSettings["ApiUsr"];
            var pwd = ConfigurationManager.AppSettings["ApiPwd"];

            var client = new RestClient(url) { Authenticator = new HttpBasicAuthenticator(usr, pwd) };
            return client;
        }

        private void GerarBoletos()
        {
            _tmrGerBol.Enabled = false;

            try
            {
                var client = GerarClient();
                var req = new RestRequest("boletos/gerar", Method.GET);
                client.Execute(req);
            }
            catch (Exception e)
            {
                EventLog.WriteEntry($"Erro: {e.Message}");
            }
            
            _tmrGerBol.Enabled = true;
        }

        private void EnviarPorEmail()
        {
            _tmrEnvEml.Enabled = false;

            try
            {
                var client = GerarClient();
                var req = new RestRequest("boletos/enviar", Method.GET);
                client.Execute(req);
            }
            catch (Exception e)
            {
                EventLog.WriteEntry($"Erro: {e.Message}");
            }
            
            _tmrEnvEml.Enabled = true;
        }

        private void SetupLogging()
        {
            
            EventLog.Source = ServiceName;
            EventLog.Log = "Application";

            ((ISupportInitialize)(EventLog)).BeginInit();

            if (!EventLog.SourceExists(EventLog.Source))
                EventLog.CreateEventSource(EventLog.Source, _log.Log);

            ((ISupportInitialize)(EventLog)).EndInit();
        }
    }
}
