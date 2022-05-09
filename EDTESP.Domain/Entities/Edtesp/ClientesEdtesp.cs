using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDTESP.Domain.Entities.Edtesp
{
    [Table("tab_clientes")]
    public class ClientesEdtesp
    {
        [Key]
        public int Id { get; set; }

        public string Letra { get; set; }

        [Column("clientes")]
        public string Nome { get; set; }

        [Column("categoria")]
        public int CategoriaId { get; set; }

        [Column("log")]
        public string Logradouro { get; set; }

        [Column("logradouro")]
        public string Endereco { get; set; }

        public string Numero { get; set; }

        [Column("compl")]
        public string Complemento { get; set; }
        
        public string Cep { get; set; }

        public string Bairro { get; set; }

        public string Cidade { get; set; }

        public string Uf { get; set; }

        public string Ddd { get; set; }

        [Column("tel_1")]
        public string Telefone { get; set; }

        [Column("tel_2")]
        public string Telefone2 { get; set; }

        [Column("tel_3")]
        public string Telefone3 { get; set; }
        
        public string Fax { get; set; }

        public string Anuncio { get; set; }

        public string Site { get; set; }

        public string Email { get; set; }

        [ForeignKey("categoriaid")]
        public virtual ClassificadosEdtesp Categoria { get; set; }
    }
}