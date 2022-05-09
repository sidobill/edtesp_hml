using System;

namespace EDTESP.Web.ViewModels.Relatorios
{
    public class ResumoVendaGeralViewModel
    {
        public int Numero { get; set; }

        public string RazaoSocial { get; set; }

        public DateTime DataCriacao { get; set; }

        public string AnoEdicao { get; set; }

        public string Time { get; set; }

        public int QtdParcela { get; set; }

        public double ValorFinal { get; set; }
    }
}