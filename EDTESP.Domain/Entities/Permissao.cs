using System.ComponentModel.DataAnnotations;

namespace EDTESP.Domain.Entities
{
    public class Permissao
    {
        [Key]
        public int PermissaoId { get; set; }

        public string Nome { get; set; }

        public string Descricao { get; set; }

        public string Categoria { get; set; }

        public string Grupo { get; set; }

        public bool Removido { get; set; }

        public string Role { get; set; }

    }
}