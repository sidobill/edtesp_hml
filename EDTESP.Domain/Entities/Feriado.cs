using System;
using System.ComponentModel.DataAnnotations;

namespace EDTESP.Domain.Entities
{
    public class Feriado
    {
        [Key]
        public int FeriadoId { get; set; }

        public DateTime Data { get; set; }

        public bool DataFixa { get; set; }

        public string Descricao { get; set; }
    }
}