using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace EDTESP.Web.ViewModels.Cadastros
{
    public class ClienteViewModel
    {
        [DisplayName("Cód.")]
        public int ClienteId { get; set; }

        [DisplayName("Tipo Pessoa")]
        public string TipoPessoa { get; set; }

        [DisplayName("CNPJ")]
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [MinLength(14, ErrorMessage = "Informe um documento válido")]
        [MaxLength(18, ErrorMessage = "Informe um documento válido")]
        [Remote("DocumentoValido","Clientes", AdditionalFields = "ClienteId,TipoPessoa", ErrorMessage = "Informe um documento válido")]
        public string Documento { get; set; }

        [DisplayName("Razão Social")]
        [Required]
        [MaxLength(150)]
        public string RazaoSocial { get; set; }

        [DisplayName("Fantasia")]
        [MaxLength(150)]
        public string Fantasia { get; set; }

        [DisplayName("CEP")]
        [Required]
        [DataType(DataType.PostalCode)]
        [MaxLength(9)]
        public string Cep { get; set; }

        [DisplayName("Endereço")]
        [Required]
        [MaxLength(150)]
        public string Endereco { get; set; }

        [DisplayName("Núm. *")]
        [MaxLength(10)]
        public string Numero { get; set; }

        [DisplayName("Compl. *")]
        [MaxLength(20)]
        public string Complemento { get; set; }

        [DisplayName("Bairro")]
        [Required]
        [MaxLength(150)]
        public string Bairro { get; set; }

        [DisplayName("Cidade")]
        [Required]
        [MaxLength(150)]
        public string Cidade { get; set; }

        [DisplayName("UF")]
        [Required]
        [MaxLength(2)]
        public string Uf { get; set; }

        [DisplayName("Telefone")]
        [Required]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(20)]
        public string Telefone { get; set; }

        [DisplayName("Celular")]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(20)]
        public string Celular { get; set; }

        [DisplayName("Fax")]
        [MaxLength(20)]
        public string Fax { get; set; }

        [DisplayName("Tel. Outros")]
        [MaxLength(20)]
        public string TelefoneOutro { get; set; }

        [DisplayName("Website")]
        [DataType(DataType.Url)]
        [MaxLength(150)]
        public string Website { get; set; }

        [DisplayName("E-mail")]
        [Required]
        [DataType(DataType.EmailAddress)]
        [MaxLength(150)]
        public string Email { get; set; }

        [DisplayName("E-mail Cobrança")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(150)]
        public string EmailCobranca { get; set; }

        [DisplayName("E-mail Outro")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(150)]
        public string EmailOutro { get; set; }

        [DisplayName("Responsável")]
        [Required]
        [MaxLength(150)]
        public string Responsavel { get; set; }

        [DisplayName("Cargo do Resposável *")]
        [MaxLength(150)]
        [Required]
        public string ResponsavelCargo { get; set; }

        [DisplayName("Obs.")]
        [MaxLength(150)]
        public string Observacao { get; set; }

        [DisplayName("CEP *")]
        public string CepCobr { get; set; }

        [DisplayName("Endereço *")]
        public string EnderecoCobr { get; set; }

        [DisplayName("Núm. *")]
        public string NumeroCobr { get; set; }

        [DisplayName("Compl. *")]
        public string ComplementoCobr { get; set; }

        [DisplayName("Bairro *")]
        public string BairroCobr { get; set; }

        [DisplayName("Cidade *")]
        public string CidadeCobr { get; set; }

        [DisplayName("UF *")]
        public string UfCobr { get; set; }

        [DisplayName("Dt. Cadastro")]
        public DateTime DataCadastro { get; set; }

        [DisplayName("Ult. Atual.")]
        public DateTime? UltimaAtualizacao { get; set; }

        [DisplayName("Status")]
        public int StatusId { get; set; }
        
        [DisplayName("Usu. Criador")]
        public string UsuarioCriador { get; set; }

        [DisplayName("Usu. Atual.")]
        public string UsuarioAtualizador { get; set; }

        [DisplayName("Status")]
        public string Status { get; set; }

        public string EmailParaBoleto => string.IsNullOrEmpty(EmailCobranca) ? Email : EmailCobranca;

        [DisplayName("Logo")]
        public string Logotipo { get; set; }

        [DisplayName("Logo")]
        public HttpPostedFileBase Logo { get; set; }

        public bool IntegradoBraslink { get; set; }
    }
}