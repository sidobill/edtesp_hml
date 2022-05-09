using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using EDTESP.Domain.Entities;

namespace EDTESP.Infrastructure.Data.Context
{
    public interface IDbContexto
    {
        IDbSet<Cargo> Cargos { get; set; }
        IDbSet<Cliente> Clientes { get; set; }
        IDbSet<CondicaoPagamento> CondicaoPagamentos  { get; set; }
        IDbSet<Contrato> Contratos { get; set; }
        IDbSet<FormaPagamento> FormasPagamentos { get; set; }
        IDbSet<GrupoUsuario> GrupoUsuarios { get; set; }
        IDbSet<PerfilUsuario> PerfisUsuario { get; set; }
        IDbSet<Permissao> Permissoes { get; set; }
        IDbSet<PermissaoGrupoUsuario> PermissoesGrupoUsuarios  { get; set; }
        IDbSet<PermissaoUsuario> PermissoesUsuarios  { get; set; }
        IDbSet<Setor> Setor { get; set; }
        IDbSet<Status> Statuses  { get; set; }
        IDbSet<Titulo> Titulos { get; set; }
        IDbSet<Usuario> Usuarios  { get; set; }
        IDbSet<Vendedor> Vendedores  { get; set; }
        IDbSet<Boleto> Boletos { get; set; }
        IDbSet<StatusContrato> StatusContratos { get; set; }
        IDbSet<ContratoEvento> ContratoEventos { get; set; }
        IDbSet<Feriado> Feriados { get; set; }
        IDbSet<Time> Times { get; set; }
        IDbSet<Empresa> Empresas { get; set; }
        IDbSet<Produto> Produtos { get; set; }
        IDbSet<MotivoSuspensao> MotivosSuspensao { get; set; }
        IDbSet<VendedorAlcada> VendedorAlcadas { get; set; }
        IDbSet<StatusVendedor> StatusVendedores { get; set; }
        IDbSet<Parametro> Parametros { get; set; }
        IDbSet<ParametroBanco> ParametroBancos { get; set; }
        IDbSet<Remessa> Remessas { get; set; }
        IDbSet<TipoBaixa> TipoBaixas { get; set; }

        IDbSet<TEntity> Set<TEntity>() where TEntity : class;
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity obj) where TEntity : class;
        
        int SaveChanges();
        void Dispose();
    }
}