using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using EDTESP.Application;
using EDTESP.Application.Interfaces;
using EDTESP.Domain.Entities;
using EDTESP.Domain.Entities.Edtesp;
using EDTESP.Domain.Repositories;
using EDTESP.Infrastructure.Data.Context;
using EDTESP.Infrastructure.Data.Repositories;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Unity;
using Unity.Lifetime;
using Unity.Mvc5;
using IUnitOfWork = EDTESP.Domain.Repositories.IUnitOfWork;

namespace EDTESP.Web
{
    public static class UnityConfig
    {
        private static UnityContainer _container;

        public static UnityContainer Container {
            get
            {
                if(_container == null)
                    _container = new UnityContainer();

                return _container;
            }
        }

        public static void RegisterComponents()
        {

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            //Container.RegisterType<DbContexto>(new PerResolveLifetimeManager());

            if (HttpContext.Current != null)
            {
                Container.RegisterType<IDbContexto, DbContexto>(new HierarchicalLifetimeManager());
                Container.RegisterType<IEdtespContexto, EdtespContexto>(new HierarchicalLifetimeManager());
            }
            else
            {
                Container.RegisterType<IDbContexto, DbContexto>(new ContainerControlledLifetimeManager());
                Container.RegisterType<IEdtespContexto, EdtespContexto>(new ContainerControlledLifetimeManager());
            }

            Container.RegisterType<IUnitOfWork, UnitOfWork>();
            Container.RegisterType<IRepositoryBase<Status>, RepositoryBase<Status>>();
            Container.RegisterType<IRepositoryBase<GrupoUsuario>, RepositoryBase<GrupoUsuario>>();
            Container.RegisterType<IRepositoryBase<Usuario>, RepositoryBase<Usuario>>();
            Container.RegisterType<IRepositoryBase<Cargo>, RepositoryBase<Cargo>>();
            Container.RegisterType<IRepositoryBase<PermissaoUsuario>, RepositoryBase<PermissaoUsuario>>();
            Container.RegisterType<IRepositoryBase<PermissaoGrupoUsuario>, RepositoryBase<PermissaoGrupoUsuario>>();
            Container.RegisterType<IRepositoryBase<Permissao>, RepositoryBase<Permissao>>();
            Container.RegisterType<IRepositoryBase<Vendedor>, RepositoryBase<Vendedor>>();
            Container.RegisterType<IRepositoryBase<FormaPagamento>, RepositoryBase<FormaPagamento>>();
            Container.RegisterType<IRepositoryBase<FormaCondicaoPagamento>, RepositoryBase<FormaCondicaoPagamento>>();
            Container.RegisterType<IRepositoryBase<CondicaoPagamento>, RepositoryBase<CondicaoPagamento>>();
            Container.RegisterType<IRepositoryBase<PerfilUsuario>, RepositoryBase<PerfilUsuario>>();
            Container.RegisterType<IRepositoryBase<Titulo>, RepositoryBase<Titulo>>();
            Container.RegisterType<IRepositoryBase<Boleto>, RepositoryBase<Boleto>>();
            Container.RegisterType<IRepositoryBase<Contrato>, RepositoryBase<Contrato>>();
            Container.RegisterType<IRepositoryBase<StatusContrato>, RepositoryBase<StatusContrato>>();
            Container.RegisterType<IRepositoryBase<ContratoEvento>, RepositoryBase<ContratoEvento>>();
            Container.RegisterType<IRepositoryBase<Cliente>, RepositoryBase<Cliente>>();
            Container.RegisterType<IRepositoryBase<Feriado>, RepositoryBase<Feriado>>();
            Container.RegisterType<IRepositoryBase<Setor>, RepositoryBase<Setor>>();
            Container.RegisterType<IRepositoryBase<Time>, RepositoryBase<Time>>();
            Container.RegisterType<IRepositoryBase<Empresa>, RepositoryBase<Empresa>>();
            Container.RegisterType<IRepositoryBase<Produto>, RepositoryBase<Produto>>();
            Container.RegisterType<IRepositoryBase<MotivoSuspensao>, RepositoryBase<MotivoSuspensao>>();
            Container.RegisterType<IRepositoryBase<VendedorAlcada>, RepositoryBase<VendedorAlcada>>();
            Container.RegisterType<IRepositoryBase<StatusVendedor>, RepositoryBase<StatusVendedor>>();
            Container.RegisterType<IRepositoryBase<ParametroBanco>, RepositoryBase<ParametroBanco>>();
            Container.RegisterType<IRepositoryBase<Parametro>, RepositoryBase<Parametro>>();
            Container.RegisterType<IRepositoryBase<Remessa>, RepositoryBase<Remessa>>();

            Container.RegisterType<IGrupoUsuarioRepository, GrupoUsuarioRepository>();
            Container.RegisterType<ICondicaoPagamentoRepository, CondicaoPagamentoRepository>();
            Container.RegisterType<IComissaoIndividualRepository, ComissaoIndividualRepository>();
            Container.RegisterType<IAdiantamentoSalarialRepository, AdiantamentoSalarialRepository>();
            Container.RegisterType<IEstornoComissaoRepository, EstornoComissaoRepository>();
            Container.RegisterType<IResumoVendaGeralRepository, ResumoVendaGeralRepository>();
            Container.RegisterType<IResumoVendaRepresentanteRepository, ResumoVendaRepresentanteRepository>();
            Container.RegisterType<ICobrancaClienteRepository, CobrancaClienteRepository>();

            Container.RegisterType<IEdtespRepositoryBase<ClientesEdtesp>, EdtespRepositoryBase<ClientesEdtesp>>();
            Container.RegisterType<IEdtespRepositoryBase<ClassificadosEdtesp>, EdtespRepositoryBase<ClassificadosEdtesp>>();

            Container.RegisterType<IAppServiceBase<Status>, AppServiceBase<Status>>();
            Container.RegisterType<IAppServiceBase<GrupoUsuario>, AppServiceBase<GrupoUsuario>>();
            Container.RegisterType<IAppServiceBase<Usuario>, AppServiceBase<Usuario>>();
            Container.RegisterType<IAppServiceBase<Cargo>, AppServiceBase<Cargo>>();
            Container.RegisterType<IAppServiceBase<PermissaoUsuario>, AppServiceBase<PermissaoUsuario>>();
            Container.RegisterType<IAppServiceBase<PermissaoGrupoUsuario>, AppServiceBase<PermissaoGrupoUsuario>>();
            Container.RegisterType<IAppServiceBase<Permissao>, AppServiceBase<Permissao>>();
            Container.RegisterType<IAppServiceBase<Vendedor>, AppServiceBase<Vendedor>>();
            Container.RegisterType<IAppServiceBase<CondicaoPagamento>, AppServiceBase<CondicaoPagamento>>();
            Container.RegisterType<IAppServiceBase<FormaPagamento>, AppServiceBase<FormaPagamento>>();
            Container.RegisterType<IAppServiceBase<FormaCondicaoPagamento>, AppServiceBase<FormaCondicaoPagamento>>();
            Container.RegisterType<IAppServiceBase<PerfilUsuario>, AppServiceBase<PerfilUsuario>>();
            Container.RegisterType<IAppServiceBase<Titulo>, AppServiceBase<Titulo>>();
            Container.RegisterType<IAppServiceBase<Boleto>, AppServiceBase<Boleto>>();
            Container.RegisterType<IAppServiceBase<Contrato>, AppServiceBase<Contrato>>();
            Container.RegisterType<IAppServiceBase<StatusContrato>, AppServiceBase<StatusContrato>>();
            Container.RegisterType<IAppServiceBase<ContratoEvento>, AppServiceBase<ContratoEvento>>();
            Container.RegisterType<IAppServiceBase<Cliente>, AppServiceBase<Cliente>>();
            Container.RegisterType<IAppServiceBase<Time>, AppServiceBase<Time>>();
            Container.RegisterType<IAppServiceBase<Setor>, AppServiceBase<Setor>>();
            Container.RegisterType<IAppServiceBase<Empresa>, AppServiceBase<Empresa>>();
            Container.RegisterType<IAppServiceBase<Produto>, AppServiceBase<Produto>>();
            Container.RegisterType<IAppServiceBase<MotivoSuspensao>, AppServiceBase<MotivoSuspensao>>();
            Container.RegisterType<IAppServiceBase<VendedorAlcada>, AppServiceBase<VendedorAlcada>>();
            Container.RegisterType<IAppServiceBase<StatusVendedor>, AppServiceBase<StatusVendedor>>();
            Container.RegisterType<IAppServiceBase<ParametroBanco>, AppServiceBase<ParametroBanco>>();
            Container.RegisterType<IAppServiceBase<Parametro>, AppServiceBase<Parametro>>();
            Container.RegisterType<IAppServiceBase<Feriado>, AppServiceBase<Feriado>>();
            Container.RegisterType<IAppServiceBase<Remessa>, AppServiceBase<Remessa>>();

            Container.RegisterType<IUsuarioAppService, UsuarioAppService>();
            Container.RegisterType<IGrupoUsuarioAppService, GrupoUsuarioAppService>();
            Container.RegisterType<ICondicaoPagamentoAppService, CondicaoPagamentoAppService>();
            Container.RegisterType<IContratoAppService, ContratoAppService>();
            Container.RegisterType<ITituloAppService, TituloAppService>();

            Container.RegisterType<IEdtespAppServiceBase<ClientesEdtesp>, EdtespAppServiceBase<ClientesEdtesp>>();
            Container.RegisterType<IEdtespAppServiceBase<ClassificadosEdtesp>, EdtespAppServiceBase<ClassificadosEdtesp>>();

            Container.RegisterType<IComissaoInvidualAppService, ComissaoIndividualService>();

            Container.RegisterType<IAdiantamentoSalarialAppService, AdiantamentoSalarialAppService>();

            Container.RegisterType<IEstornoComissaoAppService, EstornoComissaoAppService>();

            Container.RegisterType<IResumoVendaGeralAppService, ResumoVendaGeralAppService>();

            Container.RegisterType<IResumoVendaRepresentanteAppService, ResumoVendaRepresentanteAppService>();

            Container.RegisterType<ICobrancaClienteAppService, CobrancaClienteAppService>();

            Container.RegisterType<IClienteBraslinkRepository, ClienteBraslinkRepository>();

            Container.RegisterType<IIntegracaoBraslinkApp, IntegracaoBraslinkApp>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(Container));
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(Container);
            //DynamicModuleUtility.RegisterModule(typeof(UnityPer));
        }
    }
}