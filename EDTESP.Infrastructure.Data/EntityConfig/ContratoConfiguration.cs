using System.Data.Entity.ModelConfiguration;
using EDTESP.Domain.Entities;

namespace EDTESP.Infrastructure.Data.EntityConfig
{
    public class ContratoConfiguration : EntityTypeConfiguration<Contrato>
    {
        public ContratoConfiguration()
        {
            HasKey(p => p.ContratoId);
        }
    }
}