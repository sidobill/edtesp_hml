using AutoMapper;
using EDTESP.Domain.Entities;
using EDTESP.Infrastructure.CC.Util;
using EDTESP.Web.ViewModels;
using EDTESP.Web.ViewModels.Cadastros;
using EDTESP.Web.ViewModels.Operacoes;

namespace EDTESP.Web.Mappers
{
    public class ViewToDomainProfile : Profile
    {
        public ViewToDomainProfile()
        {
            CreateMap<UsuarioViewModel, Usuario>()
                .ForMember(x => x.Status, m => m.Ignore());

            CreateMap<GrupoUsuarioViewModel, GrupoUsuario>();
            CreateMap<VendedorViewModel, Vendedor>()
                .ForMember(x => x.StatusVendedor, m => m.Ignore())
                .ForMember(x => x.UsuarioCriador, m => m.Ignore())
                .ForMember(x => x.UsuarioAtualizador, m => m.Ignore())
                .AfterMap((s, d) =>
                {
                    d.Cpf = s.Cpf.ClearNumber();
                });

            CreateMap<ClienteViewModel, Cliente>()
                .ForMember(x=> x.Status, m => m.Ignore())
                .ForMember(x => x.UsuarioCriador, m => m.Ignore())
                .ForMember(x => x.UsuarioAtualizador, m => m.Ignore())
                .AfterMap((s, d) =>
                {
                    d.Documento = s.Documento.ClearNumber();
                    d.Telefone = s.Telefone.ClearNumber();
                    d.Celular = s.Celular.ClearNumber();
                    d.Fax = s.Fax.ClearNumber();
                    d.TelefoneOutro = s.TelefoneOutro.ClearNumber();
                    d.Cep = s.Cep.ClearNumber();
                    d.CepCobr = s.CepCobr.ClearNumber();
                });

            CreateMap<ProdutoViewModel, Produto>()
                .ForMember(x => x.UsuarioCriador, m => m.Ignore())
                .ForMember(x => x.UsuarioAtualizador, m => m.Ignore());
            CreateMap<FormaPagamentoViewModel, FormaPagamento>()
                .ForMember(x => x.UsuarioCriador, m => m.Ignore())
                .ForMember(x => x.UsuarioAtualizador, m => m.Ignore());
            CreateMap<CondicaoPagamentoViewModel, CondicaoPagamento>()
                .ForMember(x => x.UsuarioCriador, m => m.Ignore())
                .ForMember(x => x.UsuarioAtualizador, m => m.Ignore());
            CreateMap<ContratoViewModel, Contrato>()
                .ForMember(x => x.Numero, m => m.NullSubstitute(0))
                .ForMember(x => x.UsuarioCriador, m => m.Ignore())
                .ForMember(x => x.UsuarioAtualizador, m => m.Ignore())
                .ForMember(x => x.StatusContrato, m => m.Ignore())
                .ForMember(x => x.Empresa, m => m.Ignore())
                .ForMember(x => x.CondicaoPagamento, m => m.Ignore())
                .ForMember(x => x.FormaPagamento, m => m.Ignore())
                .ForMember(x => x.ContratoEventos, m => m.Ignore());

            CreateMap<TimeViewModel, Time>();
            CreateMap<CargoViewModel, Cargo>();
            CreateMap<SetorViewModel, Setor>();
            CreateMap<FeriadoViewModel, Feriado>();
        }
    }
}