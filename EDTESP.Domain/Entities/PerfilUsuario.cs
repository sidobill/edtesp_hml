using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDTESP.Domain.Entities
{
    public class PerfilUsuario
    {
        [Key]
        public int PerfilUsuarioId { get; set; }

        public int UsuarioId { get; set; }

        public int? VendedorId { get; set; }

        public int? CargoId { get; set; }

        public int? SetorId { get; set; }

        public DateTime DataInicio { get; set; }

        public DateTime? DataTermino { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }

        [ForeignKey("VendedorId")]
        public virtual Vendedor Vendedor { get; set; }

        [ForeignKey("CargoId")]
        public virtual Cargo Cargo { get; set; }

        [ForeignKey("SetorId")]
        public virtual Setor Setor { get; set; }
    }
}