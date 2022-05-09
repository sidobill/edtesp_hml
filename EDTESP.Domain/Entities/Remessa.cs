using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDTESP.Domain.Entities
{
    public class Remessa
    {
        [Key]
        public int RemessaId { get; set; }

        public int ParametroBancoId { get; set; }

        public DateTime DataCriacao { get; set; }

        public int UsuarioCriadorId { get; set; }

        public string NomeTxt { get; set; }

        public bool Baixada { get; set; }

        [ForeignKey("UsuarioCriadorId")]
        public virtual Usuario UsuarioCriador { get; set; }

        [ForeignKey("ParametroBancoId")]
        public virtual ParametroBanco ParametroBanco { get; set; }
    }
}