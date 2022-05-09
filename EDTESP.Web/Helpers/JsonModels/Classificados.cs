using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDTESP.Web.Helpers.JsonModels
{
    public class Classificados
    {
        public int Id { get; set; }

        public int Codigo { get; set; }
        
        public string Categoria { get; set; }

        public int Quantidade { get; set; }
    }
}