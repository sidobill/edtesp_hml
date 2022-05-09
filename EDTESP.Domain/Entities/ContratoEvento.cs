using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDTESP.Domain.Entities
{
    public class ContratoEvento
    {
        public int ContratoEventoId { get; set; }

        public int ContratoId { get; set; }

        public DateTime DataCriacao { get; set; }

        public int UsuarioCriadorId { get; set; }

        public int StatusContratoId { get; set; }

        public string Observacao { get; set; }

        [ForeignKey("ContratoId")]
        public virtual Contrato Contrato { get; set; }

        [ForeignKey("UsuarioCriadorId")]
        public virtual Usuario UsuarioCriador { get; set; }

        [ForeignKey("StatusContratoId")]
        public virtual StatusContrato StatusContrato { get; set; }
    }
}