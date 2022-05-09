using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDTESP.Web.ViewModels.Relatorios
{
    public class ComissaoIndividualViewModel
    {
        public int Numero { get; set; }

        public string RazaoSocial { get; set; }

        public string AnoEdicao { get; set; }

        public string Time { get; set; }

        public double ValorFinal { get; set; }

        public double ValorBase { get; set; }

        public double Comissao { get; set; }

        public double ValorReceber { get; set; }

        public string Status { get; set; }

        public int QtdParcela { get; set; }

        public DateTime DataCriacao { get; set; }
    }
}