using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using EDTESP.Domain.Entities;
using EDTESP.Infrastructure.Data.EntityConfig;

namespace EDTESP.Infrastructure.Data.Context
{
    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class DbContexto : DbContext, IDbContexto
    {
        public DbContexto() : base("strConn")
        {
            
        }

        public IDbSet<Cargo> Cargos { get; set; }
        public IDbSet<Cliente> Clientes { get; set; }
        public IDbSet<CondicaoPagamento> CondicaoPagamentos { get; set; }
        public IDbSet<Contrato> Contratos { get; set; }
        public IDbSet<FormaPagamento> FormasPagamentos { get; set; }
        public IDbSet<GrupoUsuario> GrupoUsuarios { get; set; }
        public IDbSet<PerfilUsuario> PerfisUsuario { get; set; }
        public IDbSet<Permissao> Permissoes { get; set; }
        public IDbSet<PermissaoGrupoUsuario> PermissoesGrupoUsuarios { get; set; }
        public IDbSet<PermissaoUsuario> PermissoesUsuarios { get; set; }
        public IDbSet<Setor> Setor { get; set; }
        public IDbSet<Status> Statuses { get; set; }
        public IDbSet<Titulo> Titulos { get; set; }
        public IDbSet<Usuario> Usuarios { get; set; }
        public IDbSet<Vendedor> Vendedores { get; set; }
        public IDbSet<Boleto> Boletos { get; set; }
        public IDbSet<StatusContrato> StatusContratos { get; set; }
        public IDbSet<ContratoEvento> ContratoEventos { get; set; }
        public IDbSet<Feriado> Feriados { get; set; }
        public IDbSet<Time> Times { get; set; }
        public IDbSet<Empresa> Empresas { get; set; }
        public IDbSet<Produto> Produtos { get; set; }
        public IDbSet<MotivoSuspensao> MotivosSuspensao { get; set; }
        public IDbSet<VendedorAlcada> VendedorAlcadas { get; set; }
        public IDbSet<StatusVendedor> StatusVendedores { get; set; }
        public IDbSet<Parametro> Parametros { get; set; }
        public IDbSet<ParametroBanco> ParametroBancos { get; set; }
        public IDbSet<Remessa> Remessas { get; set; }
        public IDbSet<TipoBaixa> TipoBaixas { get; set; }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            modelBuilder.Properties()
                .Where(p => p.Name == p.ReflectedType.Name + "Id")
                .Configure(p => p.IsKey());

            modelBuilder.Properties<string>()
                .Configure(p => p.HasColumnType("varchar"));

            modelBuilder.Properties<string>()
                .Configure(p => p.HasMaxLength(100));

            modelBuilder.Configurations.Add(new ClienteConfiguration());
            modelBuilder.Configurations.Add(new CondicaoPagamentoConfiguration());
            modelBuilder.Configurations.Add(new FormaPagamentoConfiguration());
            modelBuilder.Configurations.Add(new ContratoConfiguration());
        }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("DataCadastro") != null))
            {
                if (entry.State == EntityState.Added)
                    entry.Property("DataCadastro").CurrentValue = DateTime.Now;

                if (entry.State == EntityState.Modified)
                    entry.Property("DataCadastro").IsModified = false;
            }

            foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("DataCriacao") != null))
            {
                if (entry.State == EntityState.Added)
                    entry.Property("DataCriacao").CurrentValue = DateTime.Now;

                if (entry.State == EntityState.Modified)
                    entry.Property("DataCriacao").IsModified = false;
            }

            return base.SaveChanges();
        }
    }
}