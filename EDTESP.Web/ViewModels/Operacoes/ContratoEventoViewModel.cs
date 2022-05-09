using System;
using System.ComponentModel.DataAnnotations;

namespace EDTESP.Web.ViewModels.Operacoes
{
    public class ContratoEventoViewModel
    {
        public int ContratoEventoId { get; set; }

        public int ContratoId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime DataCriacao { get; set; }

        public int UsuarioCriadorId { get; set; }

        public int StatusContratoId { get; set; }

        public string Observacao { get; set; }
        
        public string UsuarioCriador { get; set; }

        public string StatusContrato { get; set; }
    }
}