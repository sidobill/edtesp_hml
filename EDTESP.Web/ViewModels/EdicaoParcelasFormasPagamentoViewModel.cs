using System.Collections.Generic;

namespace EDTESP.Web.ViewModels
{
    public class EdicaoParcelasFormasPagamentoViewModel
    {
        public List<ParcelaResumidoViewModel> Parcelas { get; set; }

        public List<FormaPagamentoResumidoViewModel> FormasPagamento { get; set; }
    }

    public class ParcelaResumidoViewModel
    {
        public int Id { get; set; }

        public decimal Valor { get; set; }

        public int FormaPagamentoId { get; set; }

        public int Numero { get; set; }
    }

    public class FormaPagamentoResumidoViewModel
    {
        public int Id { get; set; }

        public string Descricao { get; set; }
    }
}