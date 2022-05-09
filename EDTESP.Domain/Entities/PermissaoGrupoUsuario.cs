using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDTESP.Domain.Entities
{
    public class PermissaoGrupoUsuario
    {
        [Key, Column(Order = 0)]
        public int PermissaoId { get; set; }

        [Key, Column(Order = 1)]
        public int GrupoUsuarioId { get; set; }
        
        [ForeignKey("PermissaoId")]
        public virtual Permissao Permissao { get; set; }

        [ForeignKey("GrupoUsuarioId")]
        public virtual GrupoUsuario GrupoUsuario { get; set; }
    }
}