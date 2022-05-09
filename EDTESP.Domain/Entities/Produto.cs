using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDTESP.Domain.Entities
{
    public class Produto
    {
        [Key]
        public int ProdutoId { get; set; }

        public string Descricao { get; set; }

        public decimal PrecoBase { get; set; }

        public string Especie { get; set; }

        public string Categoria { get; set; }

        public int UsuarioCriadorId { get; set; }

        public DateTime DataCadastro { get; set; }

        public int? UsuarioAtualizadorid { get; set; }

        public DateTime? UltimaAtualizacao { get; set; }

        public bool Removido { get; set; }

        [ForeignKey("UsuarioCriadorId")]
        public virtual Usuario UsuarioCriador { get; set; }

        [ForeignKey("UsuarioAtualizadorid")]
        public virtual Usuario UsuarioAtualizador { get; set; }
    }
}