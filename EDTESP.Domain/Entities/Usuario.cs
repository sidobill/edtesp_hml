using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDTESP.Domain.Entities
{
    public class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }

        [ForeignKey("GrupoUsuario")]
        public int GrupoUsuarioId { get; set; }

        public string Login { get; set; }

        public string Senha { get; set; }

        public string Nome { get; set; }

        public string Sobrenome { get; set; }

        public string Email { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        public int UsuarioIdCriador { get; set; }

        public int? UsuarioIdAtualizador { get; set; }

        public int StatusId { get; set; }

        public bool Removido { get; set; }
        
        public virtual GrupoUsuario GrupoUsuario { get; set; }
        
        [ForeignKey("StatusId")]
        public virtual Status Status { get; set; }
        
        public virtual IEnumerable<Permissao> Permissoes { get; set; }
    }
}