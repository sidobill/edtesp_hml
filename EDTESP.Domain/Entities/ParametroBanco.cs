using System.ComponentModel.DataAnnotations;

namespace EDTESP.Domain.Entities
{
    public class ParametroBanco
    {
        [Key]
        public int ParametroBancoId { get; set; }
        
        public string Descricao { get; set; }

        public string Cnpj { get; set; }

        public string Tipo { get; set; }

        public string Banco { get; set; }

        public string Agencia { get; set; }

        public string AgenciaDv { get; set; }

        public string Conta { get; set; }

        public string ContaDv { get; set; }

        public string Cedente { get; set; }

        public string CedenteDv { get; set; }

        public decimal Juros { get; set; }

        public decimal Multa { get; set; }

        public int UltimoNossoNum { get; set; }

        public int UltimoCnab { get; set; }

        public string InfoBoleto { get; set; }

        public string Carteira { get; set; }

    }
}