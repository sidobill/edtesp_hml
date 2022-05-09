using System.Data.Entity.ModelConfiguration;
using EDTESP.Domain.Entities;

namespace EDTESP.Infrastructure.Data.EntityConfig
{
    public class FormaPagamentoConfiguration : EntityTypeConfiguration<FormaPagamento>
    {
        public FormaPagamentoConfiguration()
        {
            HasKey(p => p.FormaPagamentoId);
            HasMany(x => x.CondicaoPagamentos)
                .WithMany(x => x.FormasPagamentos)
                .Map(x =>
                {
                    x.ToTable("FormaCondicaoPagamento");
                    x.MapLeftKey("FormaPagamentoId");
                    x.MapRightKey("CondicaoPagamentoId");
                });
        }
    }
}