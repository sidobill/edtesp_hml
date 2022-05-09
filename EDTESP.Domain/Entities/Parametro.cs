using System.ComponentModel.DataAnnotations;

namespace EDTESP.Domain.Entities
{
    public class Parametro
    {
        [Key]
        public int ParametroId { get; set; }

        public string Chave { get; set; }

        public string Valor { get; set; }
    }
}