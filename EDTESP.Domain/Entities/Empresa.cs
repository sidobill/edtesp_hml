using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDTESP.Domain.Entities
{
    public class Empresa
    {
        [Key]
        public int EmpresaId { get; set; }

        public string Descricao { get; set; }

        public string Cnpj { get; set; }

        public string RazaoSocial { get; set; }

        public string Fantasia { get; set; }

        public string Inscricao { get; set; }

        public string Cep { get; set; }

        public string Endereco { get; set; }

        public string Numero { get; set; }

        public string Complemento { get; set; }

        public string Bairro { get; set; }

        public string Cidade { get; set; }

        public string Uf { get; set; }

        public string Autorizante { get; set; }

        public string Site { get; set; }

        public string Email { get; set; }

        public string Telefones { get; set; }

        public string LogoEmpresa { get; set; }

        public int? ParametroBancoId { get; set; }

        public string CaminhoFtp { get; set; }

        [ForeignKey("ParametroBancoId")]
        public virtual ParametroBanco ParametroBanco { get; set; }
    }
}