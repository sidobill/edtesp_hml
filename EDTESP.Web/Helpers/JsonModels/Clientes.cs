using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDTESP.Web.Helpers.JsonModels
{
    public class Clientes
    {
        public int Id { get; set; }

        public string Lbsid { get; set; }

        public string Letra { get; set; }

        public string Nome { get; set; }

        public int CategoriaId { get; set; }

        public string Logradouro { get; set; }

        public string Endereco { get; set; }

        public string Numero { get; set; }

        public string Complemento { get; set; }
        
        public string Cep { get; set; }

        public string Bairro { get; set; }

        public string Cidade { get; set; }

        public string Uf { get; set; }

        public string Ddd { get; set; }

        public string Telefone { get; set; }

        public string Telefone2 { get; set; }

        public string Telefone3 { get; set; }
        
        public string Fax { get; set; }

        public string Anuncio { get; set; }

        public string Site { get; set; }

        public string Email { get; set; }

        public string Insercao { get; set; }
    }
}