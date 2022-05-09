using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTESP.Domain.Entities.Relatorios
{
    public class ResumoVendaGeral
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
