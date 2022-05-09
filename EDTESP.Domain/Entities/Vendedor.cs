using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.AccessControl;

namespace EDTESP.Domain.Entities
{
    public class Vendedor
    {
        [Key]
        public int VendedorId { get; set; }

        public string Cpf { get; set; }
        
        public string Rg { get; set; }

        public string Ctps { get; set; }

        public string CtpsSerie { get; set; }
        
        public string Nome { get; set; }

        public string NomeReduzido { get; set; }

        public string Email { get; set; }

        public string Telefone { get; set; }

        public string Celular { get; set; }

        public DateTime Nascimento { get; set; }

        public string Cep { get; set; }

        public string Endereco { get; set; }

        public string Numero { get; set; }

        public string Complemento { get; set; }

        public string Bairro { get; set; }

        public string Cidade { get; set; }

        public string Uf { get; set; }

        public decimal Comissao { get; set; }

        public decimal SalarioBase { get; set; }

        public DateTime DataAdmissao { get; set; }

        public bool RecebeAdiantamento { get; set; }

        public DateTime? DataDesligamento { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        public int UsuarioCriadorId { get; set; }

        public int? UsuarioAtualizadorId { get; set; }
        
        public int StatusVendedorId { get; set; }
        
        public bool Removido { get; set; }

        public int TimeId { get; set; }

        public int CargoId { get; set; }

        public int SetorId { get; set; }

        public int VendedorAlcadaId { get; set; }

        public string Observacao { get; set; }
        
        [ForeignKey("UsuarioCriadorId")]
        public virtual Usuario UsuarioCriador { get; set; }

        [ForeignKey("UsuarioAtualizadorId")]
        public virtual Usuario UsuarioAtualizador { get; set; }

        [ForeignKey("TimeId")]
        public virtual Time Time { get; set; }

        [ForeignKey("CargoId")]
        public virtual Cargo Cargo { get; set; }

        [ForeignKey("SetorId")]
        public virtual Setor Setor { get; set; }

        [ForeignKey("VendedorAlcadaId")]
        public VendedorAlcada VendedorAlcada { get; set; }

        [ForeignKey("StatusVendedorId")]
        public StatusVendedor StatusVendedor { get; set; }
    }
}