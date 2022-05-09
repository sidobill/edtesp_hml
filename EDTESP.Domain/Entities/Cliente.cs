using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace EDTESP.Domain.Entities
{
    public class Cliente
    {
        [Key]
        public int ClienteId { get; set; }

        public string TipoPessoa { get; set; }

        public string Documento { get; set; }

        public string RazaoSocial { get; set; }

        public string Fantasia { get; set; }

        public string Cep { get; set; }

        public string Endereco { get; set; }

        public string Numero { get; set; }

        public string Complemento { get; set; }

        public string Bairro { get; set; }

        public string Cidade { get; set; }

        public string Uf { get; set; }

        public string Telefone { get; set; }

        public string Celular { get; set; }

        public string Fax { get; set; }

        public string TelefoneOutro { get; set; }

        public string Website { get; set; }

        public string Email { get; set; }

        public string EmailCobranca { get; set; }

        public string EmailOutro { get; set; }

        public string Responsavel { get; set; }

        public string ResponsavelCargo { get; set; }

        public string Observacao { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime? UltimaAtualizacao { get; set; }

        public int UsuarioCriadorId { get; set; }

        public int? UsuarioAtualizadorId { get; set; }

        public int StatusId { get; set; }

        public bool Removido { get; set; }
        
        public string CepCobr { get; set; }

        public string EnderecoCobr { get; set; }

        public string NumeroCobr { get; set; }

        public string ComplementoCobr { get; set; }

        public string BairroCobr { get; set; }

        public string CidadeCobr { get; set; }

        public string UfCobr { get; set; }

        public string Logotipo { get; set; }

        public int? SiteId { get; set; }

        [ForeignKey("StatusId")]
        public virtual Status Status { get; set; }

        [ForeignKey("UsuarioCriadorId")]
        public virtual Usuario UsuarioCriador { get; set; }

        [ForeignKey("UsuarioAtualizadorId")]
        public virtual Usuario UsuarioAtualizador { get; set; }

        [ForeignKey("ClienteId")]
        public virtual IEnumerable<Contrato> Contratos { get; set; }

        [ForeignKey("ClienteId")]
        public virtual IEnumerable<Titulo> Titulos { get; set; }
    }
}
