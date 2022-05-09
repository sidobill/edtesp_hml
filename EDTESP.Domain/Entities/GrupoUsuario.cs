using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDTESP.Domain.Entities
{
    public class GrupoUsuario
    {
        [Key]
        public int GrupoUsuarioId { get; set; }

        public string Descricao { get; set; }

        public bool Removido { get; set; }

        //[ForeignKey("UsuarioCriador")]
        public int UsuarioCriadorId { get; set; }
        
        public DateTime DataCadastro { get; set; }

        [NotMapped]
        public virtual Usuario UsuarioCriador { get; set; }

        public virtual IEnumerable<Usuario> Usuarios { get; set; }
    }
}