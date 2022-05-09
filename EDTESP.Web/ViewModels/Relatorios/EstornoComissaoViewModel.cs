using System;

namespace EDTESP.Web.ViewModels.Relatorios
{
    public class EstornoComissaoViewModel
    {
        public int Numero { get; set; }

        public string RazaoSocial { get; set; }

        public int Parcela { get; set; }

        public string FormaPgto { get; set; }

        public double ValorFinal { get; set; }

        public double Comissao { get; set; }

        public double ValorReceber { get; set; }

        public double ValorParcela { get; set; }

        public int DiasAtraso { get; set; }

        public DateTime DataVenctoReal { get; set; }
    }
}