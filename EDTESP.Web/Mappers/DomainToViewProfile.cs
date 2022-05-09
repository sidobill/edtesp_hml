using System.Linq;
using AutoMapper;
using EDTESP.Domain.Entities;
using EDTESP.Domain.Entities.Relatorios;
using EDTESP.Web.ViewModels;
using EDTESP.Web.ViewModels.Cadastros;
using EDTESP.Web.ViewModels.Financeiro;
using EDTESP.Web.ViewModels.Operacoes;
using EDTESP.Web.ViewModels.Relatorios;

namespace EDTESP.Web.Mappers
{
    public class DomainToViewProfile : Profile
    {
        public DomainToViewProfile()
        {
            CreateMap<Usuario, UsuarioViewModel>()
                .ForMember(x => x.GrupoUsuario, s => s.MapFrom(y => y.GrupoUsuario.Descricao))
                .ForMember(x => x.Status, s => s.MapFrom(y => y.Status.Descricao));

            CreateMap<Usuario, EditarUsuarioViewModel>();
            CreateMap<PermissaoUsuario, PermissaoUsuarioViewModel>();
            CreateMap<PermissaoGrupoUsuario, PermissaoGrupoUsuarioVewModel>();
            CreateMap<Permissao, PermissaoViewModel>();

            CreateMap<Time, TimeViewModel>();
            CreateMap<Cargo, CargoViewModel>();
            CreateMap<Setor, SetorViewModel>();

            CreateMap<GrupoUsuario, GrupoUsuarioViewModel>()
                .ForMember(x => x.UsuarioCriador, s => s.MapFrom(y => y.UsuarioCriador.Nome));

            CreateMap<Vendedor, VendedorViewModel>()
                .ForMember(x => x.UsuarioCriador, s => s.MapFrom(y => y.UsuarioCriador.Nome))
                .ForMember(x => x.UsuarioAtualizador, s => s.MapFrom(y => y.UsuarioAtualizador.Nome))
                .ForMember(x => x.StatusVendedor, s => s.MapFrom(y => y.StatusVendedor.Descricao))
                .ForMember(x => x.Time, s => s.MapFrom(y => y.Time.Descricao))
                .ForMember(x => x.Setor, s => s.MapFrom(y => y.Setor.Descricao))
                .ForMember(x => x.Cargo, s => s.MapFrom(y => y.Cargo.Descricao))
                .ForMember(x => x.VendedorAlcada, s => s.MapFrom(y => y.VendedorAlcada.Descricao));

            CreateMap<Cliente, ClienteViewModel>()
                .ForMember(x => x.UsuarioCriador, s => s.MapFrom(y => y.UsuarioCriador.Nome))
                .ForMember(x => x.UsuarioAtualizador, s => s.MapFrom(y => y.UsuarioAtualizador.Nome))
                .ForMember(x => x.Status, s => s.MapFrom(y => y.Status.Descricao));

            CreateMap<FormaPagamento, FormaPagamentoViewModel>()
                .ForMember(x => x.UsuarioCriador, s => s.MapFrom(y => y.UsuarioCriador.Nome))
                .ForMember(x => x.UsuarioAtualizador, s => s.MapFrom(y => y.UsuarioAtualizador.Nome));

            CreateMap<CondicaoPagamento, CondicaoPagamentoViewModel>()
                .ForMember(x => x.UsuarioCriador, s => s.MapFrom(y => y.UsuarioCriador.Nome))
                .ForMember(x => x.UsuarioAtualizador, s => s.MapFrom(y => y.UsuarioAtualizador.Nome))
                .ForMember(x => x.FormasPgto,
                    s => s.MapFrom(y => y.FormasPagamentos.Select(f => f.FormaPagamentoId).ToArray()))
                .ForMember(x => x.Formas,
                    s => s.MapFrom(y => y.FormasPagamentos.Select(f => f.Descricao).Aggregate((a, b) => a + ", " + b)));

            CreateMap<Contrato, ContratoViewModel>()
                .ForMember(x => x.UsuarioCriador, s => s.MapFrom(y => y.UsuarioCriador.Nome))
                .ForMember(x => x.UsuarioAtualizador, s => s.MapFrom(y => y.UsuarioAtualizador.Nome))
                .ForMember(x => x.Cliente, s => s.MapFrom(y => y.Cliente.RazaoSocial))
                .ForMember(x => x.Vendedor, s => s.MapFrom(y => y.Vendedor.Nome))
                .ForMember(x => x.FormaPagamento, s => s.MapFrom(y => y.FormaPagamento.Descricao))
                .ForMember(x => x.CondicaoPagamento, s => s.MapFrom(y => y.CondicaoPagamento.Descricao))
                .ForMember(x => x.StatusContrato, s => s.MapFrom(y => y.StatusContrato.Descricao))
                .ForMember(x => x.Empresa, s => s.MapFrom(y => y.Empresa.RazaoSocial));

            CreateMap<ContratoEvento, ContratoEventoViewModel>()
                .ForMember(x => x.UsuarioCriador, s => s.MapFrom(y => y.UsuarioCriador.Nome))
                .ForMember(x => x.StatusContrato, s => s.MapFrom(y => y.StatusContrato.Descricao));

            CreateMap<Titulo, TituloViewModel>()
                .ForMember(x => x.UsuarioCriador, s => s.MapFrom(y => y.UsuarioCriador.Nome))
                .ForMember(x => x.UsuarioAtualizador, s => s.MapFrom(y => y.UsuarioAtualizador.Nome))
                .ForMember(x => x.Cliente, s => s.MapFrom(y => y.Cliente.RazaoSocial))
                .ForMember(x => x.Vendedor, s => s.MapFrom(y => y.Vendedor.Nome))
                .ForMember(x => x.FormaPagamento, s => s.MapFrom(y => y.FormaPagamento.Descricao))
                .ForMember(x => x.CondicaoPagamento, s => s.MapFrom(y => y.CondicaoPagamento.Descricao))
                .ForMember(x => x.Contrato, s => s.MapFrom(y => y.Contrato.Numero))
                .ForMember(x => x.FormaPagamentoObj, s => s.MapFrom(y => y.FormaPagamento));

            CreateMap<Produto, ProdutoViewModel>()
                .ForMember(x => x.UsuarioCriador, s => s.MapFrom(y => y.UsuarioCriador.Nome))
                .ForMember(x => x.UsuarioAtualizador, s => s.MapFrom(y => y.UsuarioAtualizador.Nome));

            CreateMap<Boleto, BoletoViewModel>()
                .ForMember(x => x.UsuarioCriador, s => s.MapFrom(y => y.UsuarioCriador.Nome))
                .ForMember(x => x.Cliente, s => s.MapFrom(y => y.Cliente.RazaoSocial))
                .ForMember(x => x.Empresa, s => s.MapFrom(y => y.Empresa.RazaoSocial))
                .ForMember(x => x.CanceladoStr, s => s.MapFrom(y => y.Cancelado?"Sim":"Não"))
                .ForMember(x => x.EnviadoAoBancoStr, s => s.MapFrom(y => y.EnviadoAoBanco ? "Sim" : "Não"))
                .ForMember(x => x.ClienteObj, s => s.MapFrom(y => y.Cliente))
                .ForMember(x => x.EmpresaObj, s => s.MapFrom(y => y.Empresa));

            CreateMap<Feriado, FeriadoViewModel>();

            CreateMap<Remessa, RemessaViewModel>()
                .ForMember(x => x.ParametroBanco, s => s.MapFrom(y => y.ParametroBanco.Descricao))
                .ForMember(x => x.UsuarioCriador, s => s.MapFrom(y => y.UsuarioCriador.Nome))
                .ForMember(x => x.BaixadaStr, s => s.MapFrom(y => y.Baixada ? "Sim" : "Não"));

            CreateMap<Empresa, EmpresaViewModel>();

            CreateMap<ComissaoIndividual, ComissaoIndividualViewModel>();

            CreateMap<AdiantamentoSalarial, AdiantamentoSalarialViewModel>();

            CreateMap<EstornoComissao, EstornoComissaoViewModel>();

            CreateMap<ResumoVendaGeral, ResumoVendaGeralViewModel>();

            CreateMap<ResumoVendaRepresentante, ResumoVendaRepresentanteViewModel>();

            CreateMap<CobrancaCliente, CobrancaClienteViewModel>();
        }
    }
}