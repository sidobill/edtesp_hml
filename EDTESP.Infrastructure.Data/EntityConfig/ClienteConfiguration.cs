using EDTESP.Domain.Entities;
using System.Data.Entity.ModelConfiguration;

namespace EDTESP.Infrastructure.Data.EntityConfig
{
    public class ClienteConfiguration : EntityTypeConfiguration<Cliente>
    {
        public ClienteConfiguration()
        {
            HasKey(c => c.ClienteId);

            Property(c => c.TipoPessoa)
                .IsRequired();

            Property(c => c.Documento)
                .IsRequired()
                .HasMaxLength(14);

            Property(c => c.RazaoSocial)
                .IsRequired();

            Property(c => c.Cep)
                .IsRequired()
                .HasMaxLength(8);

            Property(c => c.Endereco)
                .IsRequired();

            Property(c => c.Numero)
                .IsRequired();

            Property(c => c.Bairro)
                .IsRequired();

            Property(c => c.Cidade)
                .IsRequired();

            Property(c => c.Uf)
                .IsRequired()
                .HasMaxLength(2);

            Property(c => c.Telefone)
                .IsRequired();

            Property(c => c.Email)
                .IsRequired();

            Property(c => c.Responsavel)
                .IsRequired();

            Property(c => c.ResponsavelCargo)
                .IsRequired();
        }
    }
}