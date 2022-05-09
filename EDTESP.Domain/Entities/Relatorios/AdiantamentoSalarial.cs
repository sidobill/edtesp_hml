using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTESP.Domain.Entities.Relatorios
{
    public class AdiantamentoSalarial
    {
        public int  Numero { get; set; }

        public string RazaoSocial { get; set; }

        public string AnoEdicao { get; set; }

        public string Time { get; set; }

        public double ValorBase { get; set; }

        public double Comissao { get; set; }

        public double ValorReceber { get; set; }

        public string Status { get; set; }

        public int QtdParcela { get; set; }

        public DateTime DataCriacao { get; set; }

        public double ValorAdiantamento { get; set; }

        public double ValorFinal { get; set; }
    }
}
