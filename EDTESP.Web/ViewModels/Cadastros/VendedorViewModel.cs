using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.EnterpriseServices;
using System.Web.Mvc;

namespace EDTESP.Web.ViewModels.Cadastros
{
    public class VendedorViewModel
    {
        [DisplayName("Cód.")]
        public int VendedorId { get; set; }

        [DisplayName("CPF.")]
        [Required]
        [MinLength(14, ErrorMessage = "Informe um Cpf válido")]
        [MaxLength(18,ErrorMessage = "Informe um Cpf válido")]
        [Remote("DocumentoExiste","Cadastros", AdditionalFields = "VendedorId", ErrorMessage = "Cpf inválido ou já utilizado")]
        public string Cpf { get; set; }
        
        [DisplayName("Nome")]
        [Required]
        public string Nome { get; set; }

        [DisplayName("Apelido *")]
        public string NomeReduzido { get; set; }

        [DisplayName("E-mail *")]
        public string Email { get; set; }

        [DisplayName("% Comissão")]
        [Required]
        [DataType(DataType.Currency)]
        public decimal Comissao { get; set; }

        [DisplayName("Dt. Cad.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime DataCadastro { get; set; }

        [DisplayName("Ult. Atual.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? DataAtualizacao { get; set; }

        [DisplayName("StatusVendedor")]
        public int StatusVendedorId { get; set; }

        [DisplayName("Status")]
        public string StatusVendedor { get; set; }

        [DisplayName("Usu. Criador")]
        public string UsuarioCriador { get; set; }

        [DisplayName("Atual. por")]
        public string UsuarioAtualizador { get; set; }

        [DisplayName("RG")]
        [MaxLength(20, ErrorMessage = "No máx. 20 caracteres")]
        [Required]
        public string Rg { get; set; }

        [DisplayName("CTPS")]
        [MaxLength(20, ErrorMessage = "No máx. 20 caracteres")]
        [Required]
        public string Ctps { get; set; }

        [DisplayName("Série")]
        [MaxLength(10, ErrorMessage = "No máx. 10 caracteres")]
        [Required]
        public string CtpsSerie { get; set; }

        [DisplayName("Telefone")]
        [DataType(DataType.PhoneNumber)]
        [Required]
        public string Telefone { get; set; }

        [DisplayName("Celular")]
        [DataType(DataType.PhoneNumber)]
        [Required]
        public string Celular { get; set; }

        [DisplayName("Dt. Nasc")]
        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? Nascimento { get; set; }

        [DisplayName("CEP")]
        [Required]
        public string Cep { get; set; }

        [DisplayName("Endereço")]
        [Required]
        public string Endereco { get; set; }

        [DisplayName("Núm. *")]
        public string Numero { get; set; }

        [DisplayName("Compl. *")]
        public string Complemento { get; set; }

        [DisplayName("Bairro")]
        [Required]
        public string Bairro { get; set; }

        [DisplayName("Cidade")]
        [Required]
        public string Cidade { get; set; }

        [DisplayName("UF")]
        [Required]
        public string Uf { get; set; }

        [DisplayName("Sal. Base R$")]
        [Required]
        [DataType(DataType.Currency)]
        public decimal SalarioBase { get; set; }

        [DisplayName("Dt. Admis.")]
        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? DataAdmissao { get; set; }

        [DisplayName("Recebe Adiant.?")]
        public bool RecebeAdiantamento { get; set; }

        [DisplayName("Data Desl.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? DataDesligamento { get; set; }

        [DisplayName("Time")]
        [Range(1,int.MaxValue, ErrorMessage = "Informe o Time")]
        public int TimeId { get; set; }

        [DisplayName("Cargo")]
        [Range(1, int.MaxValue, ErrorMessage = "Informe o Cargo")]
        public int CargoId { get; set; }

        [DisplayName("Setor")]
        [Range(1, int.MaxValue, ErrorMessage = "Informe o Setor")]
        public int SetorId { get; set; }

        [DisplayName("Alçada")]
        [Range(1, int.MaxValue, ErrorMessage = "Informe o Alçada")]
        public int VendedorAlcadaId { get; set; }

        [DisplayName("Time")]
        public string Time { get; set; }

        [DisplayName("Setor")]
        public string Setor { get; set; }

        [DisplayName("Cargo")]
        public string Cargo { get; set; }

        [DisplayName("Alçada")]
        public string VendedorAlcada { get; set; }

        [DisplayName("Obs. *")]
        public string Observacao { get; set; }

    }
}