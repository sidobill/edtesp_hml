using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.AccessControl;

namespace EDTESP.Domain.Entities
{
    public class PermissaoUsuario
    {
        [Key, Column(Order = 0)]
        public int PermissaoId { get; set; }

        [Key, Column(Order = 1)]
        public int UsuarioId { get; set; }
        
        [ForeignKey("PermissaoId")]
        public virtual Permissao Permissao { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }

    }
}