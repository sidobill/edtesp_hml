using System;
using System.Collections.Generic;
using System.IO;
using EDTESP.Domain.Entities;

namespace EDTESP.Application.Interfaces
{
    public interface ITituloAppService : IAppServiceBase<Titulo>
    {
        void GravarParcela(Titulo tit, Contrato contr, bool persiste = true);

        void GerarBoleto(int tituloId, int usuarioId);

        void GerarBoletos(List<int> boletosId, int modalidadeId, int usuarioId, string folderpath);

        void BaixarboletosNaoGerados(string folderpath);

        void EnviarBoletosAoCliente(string folderpath);

        void EnviarBoletosContrato(int contratoId, string folderpath);

        void ProcessarBoletosGnet();

        void EnviarPorEmail(string folderpath, int boletoId);

        void AlterarVencto(int tituloId, DateTime novoVencto, int usuarioId, decimal ? valorTotal);

        void CancelarBoleto(int boletoId, int usuarioId, bool novoBoleto = false);

        void BaixarTitulo(int tituloId, int usuarioId, DateTime dataBaixa, bool naoGerarExcecao = false);

        void RemoverTitulos(IEnumerable<Titulo> tits, int usuarioId, bool persist = false);

        bool RemoverBoleto(int boletoId);

        string ProcessarArquivoRetorno(Stream stream, int usuarioId);

        Dictionary<bool, string> MarcarComoNaoBaixado(int tituloId, int usuarioId);

        List<Titulo> RetornarTitulosPorContrato(int contratoId);

        void CancelarTitulo(int tituloId, int usuarioId);
    }
}