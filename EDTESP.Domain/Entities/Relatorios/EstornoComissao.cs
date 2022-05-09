using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTESP.Domain.Entities.Relatorios
{
    public class EstornoComissao
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
