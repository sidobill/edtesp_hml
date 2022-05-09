using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTESP.Domain.Entities.Relatorios
{
    public class CobrancaCliente
    {
        public int Numero { get; set; }

        public string RazaoSocial { get; set; }

        public string Documento { get; set; }

        public string EnderecoCobr { get; set; }

        public string BairroCobr { get; set; }

        public string CidadeCobr { get; set; }

        public string Telefone { get; set; }

        public string Responsavel { get; set; }

        public string Vendedor { get; set; }

        public int Parcela { get; set; }

        public DateTime DataVenctoReal { get; set; }

        public double ValorParcela { get; set; }

        public string CepCobr { get; set; }

        public int QuantidadeTotalParcela { get; set; }

        public string NumeroCobr { get; set; }

        public int ClienteId { get; set; }

        public string Fantasia { get; set; }
    }
}
