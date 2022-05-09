using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using EDTESP.Domain.Entities.Edtesp;
using EDTESP.Infrastructure.Data.EntityConfig;

namespace EDTESP.Infrastructure.Data.Context
{
    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class EdtespContexto : DbContext, IEdtespContexto
    {
        public IDbSet<ClientesEdtesp> ClientesEdtesp { get; set; }
        public IDbSet<ClassificadosEdtesp> ClassificadosEdtesp { get; set; }

        public EdtespContexto() : base("edtespConn")
        {

        }

        IDbSet<TEntity> IEdtespContexto.Set<TEntity>()
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
        }
    }
}