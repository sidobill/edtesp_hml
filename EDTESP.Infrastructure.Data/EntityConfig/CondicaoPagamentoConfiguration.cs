using System.Data.Entity.ModelConfiguration;
using EDTESP.Domain.Entities;

namespace EDTESP.Infrastructure.Data.EntityConfig
{
    public class CondicaoPagamentoConfiguration : EntityTypeConfiguration<CondicaoPagamento>
    {
        public CondicaoPagamentoConfiguration()
        {
            HasKey(p => p.CondicaoPagamentoId);
            HasMany(x => x.FormasPagamentos)
                .WithMany(x => x.CondicaoPagamentos)
                .Map(x =>
                {
                    x.ToTable("FormaCondicaoPagamento");
                    x.MapLeftKey("CondicaoPagamentoId");
                    x.MapRightKey("FormaPagamentoId");
                });
        }
    }
}