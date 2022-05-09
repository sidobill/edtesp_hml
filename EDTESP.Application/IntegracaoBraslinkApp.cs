using Edtesp.Ftp;
using EDTESP.Application.Interfaces;
using EDTESP.Domain.Entities;
using EDTESP.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTESP.Application
{
    public class IntegracaoBraslinkApp : IIntegracaoBraslinkApp
    {
        private readonly IAppServiceBase<Cliente> _appsvcCli;
        private readonly IClienteBraslinkRepository _clienteBraslinkRepository;

        public IntegracaoBraslinkApp(IAppServiceBase<Cliente> appsvcCli, IClienteBraslinkRepository clienteBraslinkRepository)
        {
            _appsvcCli = appsvcCli;
            _clienteBraslinkRepository = clienteBraslinkRepository;
        }

        public bool AlterarClienteParaBraslink(ClienteBraslink clienteBraslink, string caminhoBase)
        {
            if (!string.IsNullOrEmpty(clienteBraslink.WebSite))
                clienteBraslink.WebSite = clienteBraslink.WebSite.Replace("http://", "").Replace("https://", "").Replace("http//", "").Replace("https//", "");

            var objBusca = this.RetornarClienteBraslinkPeloId(clienteBraslink.CodigoEdtesp);

            clienteBraslink.Logotipo = this.RetornarImagemAndEnviarFtp(clienteBraslink.CodigoEdtesp, objBusca.Logotipo, caminhoBase);

            return _clienteBraslinkRepository.Atualizar(clienteBraslink);
        }

        public bool ExcluirClienteBraslink(int clienteId)
        {
            var cliente = _appsvcCli.Get(clienteId);

            return _clienteBraslinkRepository.ExcluirClienteBraslink(clienteId);
        }

        public bool IntegrarClienteParaBraslink(int clienteId, string caminhoBase)
        {
            var cliente = _appsvcCli.Get(clienteId);

            if (cliente != null && cliente.ClienteId > 0)
            {
                var clienteBraslink = new ClienteBraslink();
                clienteBraslink.Bairro = cliente.Bairro;
                clienteBraslink.Categoria = "582";
                clienteBraslink.Cep = cliente.Cep;
                clienteBraslink.Cidade = cliente.Cidade;
                clienteBraslink.Cliente = cliente.Fantasia;
                clienteBraslink.Compl = cliente.Complemento;
                clienteBraslink.Ddd = "";
                clienteBraslink.Telefone = cliente.Telefone;
                clienteBraslink.Tipo = "P";
                clienteBraslink.Uf = cliente.Uf;
                clienteBraslink.Email = cliente.Email;
                clienteBraslink.Logotipo = cliente.Logotipo;
                clienteBraslink.Logradouro = cliente.Endereco;
                clienteBraslink.CodigoEdtesp = cliente.ClienteId;
                clienteBraslink.Numero = cliente.Numero;

                if (!string.IsNullOrEmpty(cliente.Website))
                    clienteBraslink.WebSite = cliente.Website.Replace("http://", "").Replace("https://", "").Replace("http//", "").Replace("https//", "");
                else
                    clienteBraslink.WebSite = cliente.Website;

                clienteBraslink.Letra = cliente.Fantasia.First().ToString().ToUpper();
                clienteBraslink.Logotipo = this.RetornarImagemAndEnviarFtp(clienteId, "", caminhoBase);

                return _clienteBraslinkRepository.Inserir(clienteBraslink);
            }

            return false;
        }

        public List<int> RetornarClientesIntegrados()
        {
            return _clienteBraslinkRepository.RetornarClientesIntegrados();
        }

        public ClienteBraslink RetornarClienteBraslinkPeloId(int clienteId)
        {
            return _clienteBraslinkRepository.RetornarPeloClienteId(clienteId);
        }

        private string RetornarImagemAndEnviarFtp(int codigoEdtesp, string nomeImagemExistente, string caminhoBase)
        {
            if (nomeImagemExistente == null)
                nomeImagemExistente = "";

            var nomeImagemCliente = "";

            if(codigoEdtesp > 0)
            {
                var cliente = _appsvcCli.Get(codigoEdtesp);
                nomeImagemCliente = cliente.Logotipo;
            }

            if (!string.IsNullOrEmpty(nomeImagemCliente))
            {
                if (nomeImagemCliente != nomeImagemExistente)
                    Ftp.UploadSingeFile(caminhoBase + "\\" + nomeImagemCliente);

            }

            return nomeImagemCliente;
        }
    }
}
