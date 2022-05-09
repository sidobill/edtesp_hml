using System.Collections.Generic;
using EDTESP.Domain.Entities;
using MySql.Data.Entity;

namespace EDTESP.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<EDTESP.Infrastructure.Data.Context.DbContexto>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.Entity.MySqlMigrationSqlGenerator());
            CodeGenerator = new MySqlMigrationCodeGenerator();
        }
        
        protected override void Seed(EDTESP.Infrastructure.Data.Context.DbContexto context)
        {
            var pg = context.PermissoesGrupoUsuarios.ToList();
            pg.ForEach(x => context.PermissoesGrupoUsuarios.Remove(x));

            var pu = context.PermissoesUsuarios.ToList();
            pu.ForEach(x => context.PermissoesUsuarios.Remove(x));

            var ps = context.Permissoes.ToList();
            ps.ForEach(x => context.Permissoes.Remove(x));

            context.SaveChanges();

            var pi = 1;
            var prms = new Permissao[]
            {
                new Permissao
                {
                    PermissaoId = ++pi,
                    Nome = "Gerenciar Usuários",
                    Descricao = "Visualizar a lista de usuários do sistema",
                    Categoria = "Cadastros",
                    Grupo = "Usuários e Grupos",
                    Removido = false,
                    Role = "GERUSU"
                },
                new Permissao
                {
                    PermissaoId = ++pi,
                    Nome = "Modificar Usuários",
                    Descricao = "Permite adicionar, editar ou remover usuários do sistema",
                    Categoria = "Cadastros",
                    Grupo = "Usuários e Grupos",
                    Removido = false,
                    Role = "MODUSU"
                },
                new Permissao
                {
                    PermissaoId = ++pi,
                    Nome = "Gerenciar Grupos de usuário",
                    Descricao = "Visualizar a lista de grupos de usuário do sistema",
                    Categoria = "Cadastros",
                    Grupo = "Usuários e Grupos",
                    Removido = false,
                    Role = "GERGRP"
                },
                new Permissao
                {
                    PermissaoId = ++pi,
                    Nome = "Modificar Grupos de usuário",
                    Descricao = "Permite adicionar, editar ou remover grupos de usuário do sistema",
                    Categoria = "Cadastros",
                    Grupo = "Usuários e Grupos",
                    Removido = false,
                    Role = "MODGRP"
                },
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Gerenciar Clientes",
                    Descricao = "Visualizar a lista de clientes do sistema",
                    Categoria = "Cadastros",
                    Grupo = "Clientes",
                    Removido = false,
                    Role = "GERCLI"
                },
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Modificar Clientes",
                    Descricao = "Permite adicionar, editar ou remover clientes do sistema",
                    Categoria = "Cadastros",
                    Grupo = "Clientes",
                    Removido = false,
                    Role = "MODCLI"
                },
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Gerenciar Vendedores",
                    Descricao = "Visualizar a lista de vendedores do sistema",
                    Categoria = "Cadastros",
                    Grupo = "Vendedores",
                    Removido = false,
                    Role = "GERVND"
                },
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Modificar Vendedores",
                    Descricao = "Permite adicionar, editar ou remover vendedores do sistema",
                    Categoria = "Cadastros",
                    Grupo = "Vendedores",
                    Removido = false,
                    Role = "MODVND"
                },
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Gerenciar Formas de Pgto.",
                    Descricao = "Visualizar a lista de Formas de Pgto. do sistema",
                    Categoria = "Cadastros",
                    Grupo = "Formas/Cond. de pgto.",
                    Removido = false,
                    Role = "GERFPG"
                },
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Modificar Formas de Pgto.",
                    Descricao = "Permite adicionar, editar ou remover Formas de Pgto. do sistema",
                    Categoria = "Cadastros",
                    Grupo = "Formas/Cond. de pgto.",
                    Removido = false,
                    Role = "MODFPG"
                },
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Gerenciar Condições de Pgto.",
                    Descricao = "Visualizar a lista de Condições de Pgto. do sistema",
                    Categoria = "Cadastros",
                    Grupo = "Formas/Cond. de pgto.",
                    Removido = false,
                    Role = "GERCPG"
                },
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Modificar Condições de pgto",
                    Descricao = "Permite adicionar, editar ou remover Condições de Pgto. do sistema",
                    Categoria = "Cadastros",
                    Grupo = "Formas/Cond. de pgto.",
                    Removido = false,
                    Role = "MODCPG"
                },
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Gerenciar Times",
                    Descricao = "Visualizar a lista de Times",
                    Categoria = "Cadastros",
                    Grupo = "Times,Setores e Cargos",
                    Removido = false,
                    Role = "GERTIM"
                },
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Modificar Times",
                    Descricao = "Permite adicionar, editar ou remover Times",
                    Categoria = "Cadastros",
                    Grupo = "Times,Setores e Cargos",
                    Removido = false,
                    Role = "MODTIM"
                },
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Gerenciar Feriados",
                    Descricao = "Visualizar a lista de Feriados",
                    Categoria = "Cadastros",
                    Grupo = "Outros",
                    Removido = false,
                    Role = "GERFER"
                },
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Modificar Feriados",
                    Descricao = "Permite adicionar, editar ou remover Feriados",
                    Categoria = "Cadastros",
                    Grupo = "Outros",
                    Removido = false,
                    Role = "MODFER"
                },
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Gerenciar Cargos",
                    Descricao = "Visualizar a lista de Cargos",
                    Categoria = "Cadastros",
                    Grupo = "Times,Setores e Cargos",
                    Removido = false,
                    Role = "GERCRG"
                },
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Modificar Cargos",
                    Descricao = "Permite adicionar, editar ou remover Cargos",
                    Categoria = "Cadastros",
                    Grupo = "Times,Setores e Cargos",
                    Removido = false,
                    Role = "MODCRG"
                },
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Gerenciar Setores",
                    Descricao = "Visualizar a lista de Setores",
                    Categoria = "Cadastros",
                    Grupo = "Times,Setores e Cargos",
                    Removido = false,
                    Role = "GERSET"
                },
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Modificar Setores",
                    Descricao = "Permite adicionar, editar ou remover Setores",
                    Categoria = "Cadastros",
                    Grupo = "Times,Setores e Cargos",
                    Removido = false,
                    Role = "MODSET"
                },
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Gerenciar Produtos",
                    Descricao = "Visualizar a lista de Produtos",
                    Categoria = "Cadastros",
                    Grupo = "Produtos",
                    Removido = false,
                    Role = "GERPRD"
                },
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Modificar Produtos",
                    Descricao = "Permite adicionar, editar ou remover Produtos",
                    Categoria = "Cadastros",
                    Grupo = "Produtos",
                    Removido = false,
                    Role = "MODPRD"
                },

                //Contratos
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Gerenciar Contratos.",
                    Descricao = "Visualizar a Lista de Contratos",
                    Categoria = "Operações",
                    Grupo = "Contratos",
                    Removido = false,
                    Role = "GERCTR"
                },
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Modificar Contratos",
                    Descricao = "Permite adicionar e editar Contratos do sistema",
                    Categoria = "Operações",
                    Grupo = "Contratos",
                    Removido = false,
                    Role = "MODCTR"
                },
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Aprovar/Reprovar Contratos",
                    Descricao = "Permite Aprovar/Reprovar Contratos",
                    Categoria = "Operações",
                    Grupo = "Contratos",
                    Removido = false,
                    Role = "APVCTR"
                },
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Baixar Contratos",
                    Descricao = "Permite fazer donwload de Contratos",
                    Categoria = "Operações",
                    Grupo = "Contratos",
                    Removido = false,
                    Role = "DOWCTR"
                },
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Suspender Contratos",
                    Descricao = "Permite suspender contratos",
                    Categoria = "Operações",
                    Grupo = "Contratos",
                    Removido = false,
                    Role = "SUSCTR"
                },

                //Financeiro
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Gerenciar Financeiro",
                    Descricao = "Permite visualizar o Contas a Receber",
                    Categoria = "Financeiro",
                    Grupo = "Contas a Receber",
                    Removido = false,
                    Role = "GERTIT"
                },
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Alterar Data Vencto.",
                    Descricao = "Alterar Data Vencto.",
                    Categoria = "Financeiro",
                    Grupo = "Contas a Receber",
                    Removido = false,
                    Role = "ALTTIT"
                },
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Baixar Titulo",
                    Descricao = "Permite marcar o titulo como pago",
                    Categoria = "Financeiro",
                    Grupo = "Contas a Receber",
                    Removido = false,
                    Role = "BXATIT"
                },

                //Boletos
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Gerar Boletos",
                    Descricao = "Permite Gerar Boletos",
                    Categoria = "Financeiro",
                    Grupo = "Boletos",
                    Removido = false,
                    Role = "ADDBOL"
                },
                new Permissao {
                    PermissaoId = ++pi,
                    Nome = "Gerar CNAB",
                    Descricao = "Permite Gerar CNAB",
                    Categoria = "Financeiro",
                    Grupo = "Boletos",
                    Removido = false,
                    Role = "ADDCNB"
                },
            };
            context.Permissoes.AddOrUpdate(prms);

            var stts = new[]
            {
                new Status
                {
                    StatusId = 0,
                    Descricao = "Inativo"
                },
                new Status
                {
                    StatusId = 1,
                    Descricao = "Ativo"
                }
            };
            context.Statuses.AddOrUpdate(stts);

            var grp = new GrupoUsuario
            {
                GrupoUsuarioId = 1,
                Descricao = "Administradores",
                UsuarioCriadorId = 1,
                DataCadastro = new DateTime(2018,11,1,0,0,0)
            };
            context.GrupoUsuarios.AddOrUpdate(grp);

            var lst = prms.Select(x => new PermissaoGrupoUsuario
            {
                PermissaoId = x.PermissaoId,
                GrupoUsuarioId = 1
            }).ToArray();
            context.PermissoesGrupoUsuarios.AddOrUpdate(lst.ToArray());

            var usr = new Usuario
            {
                UsuarioId = 1,
                GrupoUsuarioId = 1,
                Login = "admin",
                Senha = "uyO/ywT5CVQ=",
                Nome = "Administrador",
                Sobrenome = "EDTESP",
                Email = "artur@up4all.com.br",
                DataCadastro = new DateTime(2018,11,1,0,0,0),
                UsuarioIdCriador = 0,
                StatusId = 1,
                Removido = false
            };
            context.Usuarios.AddOrUpdate(usr);

            var cstts = new[]
            {
                new StatusContrato
                {
                    StatusContratoId = 0,
                    Descricao = "Em Análise"
                },
                new StatusContrato
                {
                    StatusContratoId = 1,
                    Descricao = "Aprovado"
                },
                new StatusContrato
                {
                    StatusContratoId = 2,
                    Descricao = "Reprovado"
                },
                new StatusContrato
                {
                    StatusContratoId = 3,
                    Descricao = "Cancelado"
                },
                new StatusContrato
                {
                    StatusContratoId = 4,
                    Descricao = "Suspenso"
                }
            };
            context.StatusContratos.AddOrUpdate(cstts);

            var times = new[]
            {
                new Time
                {
                    TimeId = 1,
                    Descricao = "Time 1",
                    Removido = false
                },
                new Time
                {
                    TimeId = 2,
                    Descricao = "Time 2",
                    Removido = false
                },
                new Time
                {
                    TimeId = 3,
                    Descricao = "Time 3",
                    Removido = false
                },
                new Time
                {
                    TimeId = 4,
                    Descricao = "Time 4",
                    Removido = false
                },
                new Time
                {
                    TimeId = 5,
                    Descricao = "Time 5",
                    Removido = false
                },
            };
            context.Times.AddOrUpdate(times);

            var motivossusp = new[]
            {
                new MotivoSuspensao
                {
                    MotivoSuspensaoId = 1,
                    Descricao = "Desacordo Comercial"
                },
                new MotivoSuspensao
                {
                    MotivoSuspensaoId = 2,
                    Descricao = "Solicitação do Cliente"
                },
                new MotivoSuspensao
                {
                    MotivoSuspensaoId = 3,
                    Descricao = "Falta de Pagamento"
                },
                new MotivoSuspensao
                {
                    MotivoSuspensaoId = 4,
                    Descricao = "Extra Judicial"
                },
            };
            context.MotivosSuspensao.AddOrUpdate(motivossusp);

            var alcs = new[]
            {
                new VendedorAlcada
                {
                    VendedorAlcadaId = 1,
                    Descricao = "Vendedor/Repres."
                },
                new VendedorAlcada
                {
                    VendedorAlcadaId = 2,
                    Descricao = "Supervisor"
                },
                new VendedorAlcada
                {
                    VendedorAlcadaId = 3,
                    Descricao = "Gerente"
                },
                new VendedorAlcada
                {
                    VendedorAlcadaId = 4,
                    Descricao = "Diretoria"
                },
            };
            context.VendedorAlcadas.AddOrUpdate(alcs);

            var crgs = new[]
            {
                new Cargo
                {
                    CargoId = 1,
                    Descricao = "Vendedor",
                    Removido = false
                },
            };
            context.Cargos.AddOrUpdate(crgs);

            var sets = new[]
            {
                new Setor()
                {
                    SetorId = 1,
                    Descricao = "Vendas",
                    Removido = false
                },
            };
            context.Setor.AddOrUpdate(sets);

            var vstts = new[]
            {
                new StatusVendedor
                {
                    StatusVendedorId = 0,
                    Descricao = "Inativo"
                },
                new StatusVendedor
                {
                    StatusVendedorId = 1,
                    Descricao = "Ativo"
                },
                new StatusVendedor
                {
                    StatusVendedorId = 2,
                    Descricao = "Afastado"
                },
                new StatusVendedor
                {
                    StatusVendedorId = 3,
                    Descricao = "Em Férias"
                },
                new StatusVendedor
                {
                    StatusVendedorId = 4,
                    Descricao = "Desligado"
                },
            };
            context.StatusVendedores.AddOrUpdate(vstts);

            var formas = new[]
            {
                new FormaPagamento
                {
                    FormaPagamentoId = 1,
                    Descricao = "Dinheiro",
                    DataCadastro = new DateTime(2018, 11, 1, 0, 0, 0),
                    Removido = false,
                    GeraBoleto = false,
                    UsuarioCriadorId = 1,
                },
                new FormaPagamento
                {
                    FormaPagamentoId = 2,
                    Descricao = "Boleto",
                    DataCadastro = new DateTime(2018, 11, 1, 0, 0, 0),
                    Removido = false,
                    GeraBoleto = true,
                    UsuarioCriadorId = 1,
                },
                new FormaPagamento
                {
                    FormaPagamentoId = 3,
                    Descricao = "Cheque",
                    DataCadastro = new DateTime(2018, 11, 1, 0, 0, 0),
                    Removido = false,
                    GeraBoleto = false,
                    UsuarioCriadorId = 1,
                },
            };
            context.FormasPagamentos.AddOrUpdate(formas);

            var conds = new[]
            {
                new CondicaoPagamento
                {
                    CondicaoPagamentoId = 1,
                    Descricao = "À Vista",
                    Modelo = "0",
                    UsuarioCriadorId = 1,
                    DataCadastro = new DateTime(2018, 11, 1, 0, 0, 0),
                    FormasPagamentos = formas,
                    Removido = false,
                }
            };
            context.CondicaoPagamentos.AddOrUpdate(conds);

            var vnds = new[]
            {
                new Vendedor
                {
                    VendedorId = 1,
                    Nome = "Jose Vendedor",
                    NomeReduzido = "Vendedor",
                    StatusVendedorId = 1,
                    Cpf = "11111111111",
                    SetorId = 1,
                    CargoId = 1,
                    TimeId = 1,
                    UsuarioCriadorId = 1,
                    VendedorAlcadaId = 1,
                    DataAdmissao = new DateTime(2018,1,1,0,0,0),
                    Removido = false
                },
            };
            context.Vendedores.AddOrUpdate(vnds);

            var prds = new[]
            {
                new Produto
                {
                    ProdutoId = 1,
                    Categoria = "Teste",
                    Especie = "teste",
                    PrecoBase = 1000,
                    Descricao = "Produto de teste",
                    Removido = false,
                    UsuarioCriadorId = 1
                },
            };
            context.Produtos.AddOrUpdate(prds);
            
            //var prmsbncs = new[]
            //{
            //    new ParametroBanco
            //    {
            //        ParametroBancoId = 1,
            //        Banco = "0237",
            //        Descricao = "Bradesco Carteira 09 (Matriz)",
            //        Tipo = "C",
            //        Agencia = "0091",
            //        AgenciaDv = "4",
            //        Conta = "0179568",
            //        ContaDv = "6",
            //        Cedente = "4092614",
            //        CedenteDv = "",
            //        Carteira = "09",
            //        Juros = 0.33m,
            //        Multa = 2,
            //        UltimoCnab = context.Remessas.Count(x => x.ParametroBancoId == 1),
            //        UltimoNossoNum = context.Boletos.Count(x => x.ModalidadeId == 1),
            //        InfoBoleto = "",
            //    },
            //    new ParametroBanco
            //    {
            //        ParametroBancoId = 2,
            //        Banco = "9999",
            //        Descricao = "Gerencia.Net API",
            //        Tipo = "A",
            //        Agencia = "Client_Id_bef5b76cc95c813321ea64eefaff67663eb42286",
            //        AgenciaDv = "",
            //        Conta = "Client_Secret_d2531588bed3a1cff8d10b74684c54d491b64e40",
            //        ContaDv = "",
            //        Cedente = "",
            //        CedenteDv = "",
            //        Carteira = "",
            //        Juros = 0.33m,
            //        Multa = 2,
            //        UltimoCnab = context.Remessas.Count(x => x.ParametroBancoId == 2),
            //        UltimoNossoNum = context.Boletos.Count(x => x.ModalidadeId == 2),
            //        InfoBoleto = "",
            //    },
            //    new ParametroBanco
            //    {
            //        ParametroBancoId = 3,
            //        Banco = "0237",
            //        Descricao = "Bradesco Carteira 09 (Matriz Nova)",
            //        Tipo = "C",
            //        Agencia = "0091",
            //        AgenciaDv = "4",
            //        Conta = "0263288",
            //        ContaDv = "8",
            //        Cedente = "",
            //        CedenteDv = "",
            //        Carteira = "09",
            //        Juros = 0.33m,
            //        Multa = 2,
            //        UltimoCnab = context.Remessas.Count(x => x.ParametroBancoId == 3),
            //        UltimoNossoNum = context.Boletos.Count(x => x.ModalidadeId == 3),
            //        InfoBoleto = "",
            //    }
            //};
            //context.ParametroBancos.AddOrUpdate(prmsbncs);

            var emps = new[]
            {
                new Empresa
                {
                    EmpresaId = 1,
                    Descricao = "EDTESP",
                    RazaoSocial = "EDTESP ED NACIONAL DE GUIAS E LISTAS",
                    Fantasia = "EDTESP ED NACIONAL DE GUIAS E LISTAS",
                    Cnpj = "04572621000109",
                    Autorizante = "",
                    Inscricao = "",
                    Cep = "02013010",
                    Endereco = "Rua Henrique Bernardelli",
                    Numero = "136",
                    Complemento = "Sala 92",
                    Bairro = "Santana",
                    Cidade = "São Paulo",
                    Uf = "SP",
                    Email = "edtespcontrato@gmail.com",
                    Site = "http://www.listadeenderecos.info",
                    Telefones = "(11) 3256-6516 (11) 3256-2285 (11) 3256-2353 (11) 3256-0345",
                    LogoEmpresa = "edtesp.png",
                    ParametroBancoId = 1,
                    CaminhoFtp = "/filhotes/listadeenderecos.info/www/anuncios"
                },
                new Empresa
                {
                    EmpresaId = 2,
                    Descricao = "CLANESP",
                    RazaoSocial = "Clanesp Editora Nacional de Guias e Listas Ltda.",
                    Fantasia = "CLANESP ED NACIONAL DE GUIAS E LISTAS",
                    Cnpj = "04572621000109",
                    Autorizante = "",
                    Inscricao = "",
                    Cep = "02013010",
                    Endereco = "Rua Henrique Bernardelli",
                    Numero = "136",
                    Complemento = "Sala 92",
                    Bairro = "Santana",
                    Cidade = "São Paulo",
                    Uf = "SP",
                    Email = "clanespcontrato@gmail.com",
                    Site = "http://www.clanesp.com.br",
                    Telefones = "(11) 3256-6516 (11) 3256-2285 (11) 3256-2353 (11) 3256-0345",
                    LogoEmpresa = "clanesp.png",
                    ParametroBancoId = 1,
                    CaminhoFtp = "/filhotes/clanesp.com.br/www/logos"
                },
                new Empresa
                {
                    EmpresaId = 3,
                    Descricao = "GUIAFAZ",
                    RazaoSocial = "Guiafaz Editora Nacional de Guias e Listas Ltda.",
                    Fantasia = "GUIAFAZ ED NACIONAL DE GUIAS E LISTAS",
                    Cnpj = "04572621000109",
                    Autorizante = "",
                    Inscricao = "",
                    Cep = "02013010",
                    Endereco = "Rua Henrique Bernardelli",
                    Numero = "136",
                    Complemento = "Sala 92",
                    Bairro = "Santana",
                    Cidade = "São Paulo",
                    Uf = "SP",
                    Email = "guiafazcontrato@gmail.com",
                    Site = "http://www.guiafaz.com.br",
                    Telefones = "(11) 3256-6516 (11) 3256-2285 (11) 3256-2353 (11) 3256-0345",
                    LogoEmpresa = "guiafaz.png",
                    ParametroBancoId = 1,
                    CaminhoFtp = "/filhotes/guiafaz.com.br/www/anuncios"
                },
                new Empresa
                {
                    EmpresaId = 4,
                    Descricao = "EDITORA NACIONAL",
                    RazaoSocial = "Editora Nacional de Guias e Listas Ltda.",
                    Fantasia = "EDITORA NACIONAL DE GUIAS E LISTAS",
                    Cnpj = "04572621000109",
                    Autorizante = "",
                    Inscricao = "",
                    Cep = "02013010",
                    Endereco = "Rua Henrique Bernardelli",
                    Numero = "136",
                    Complemento = "Sala 92",
                    Bairro = "Santana",
                    Cidade = "São Paulo",
                    Uf = "SP",
                    Email = "editnac@gmail.com",
                    Site = "http://www.melhoresdoramo.com",
                    Telefones = "(11) 3256-6516 (11) 3256-2285 (11) 3256-2353 (11) 3256-0345",
                    LogoEmpresa = "nacional.png",
                    ParametroBancoId = 1,
                    CaminhoFtp = "/filhotes/editoranacional.net/www/logos"
                },
                new Empresa
                {
                    EmpresaId = 5,
                    Descricao = "LISTAFACIL",
                    RazaoSocial = "Listafacil Editora Nacional de Guias e Listas Ltda",
                    Fantasia = "LISTAFACIL ED NACIONAL DE GUIAS E LISTAS",
                    Cnpj = "04572621000109",
                    Autorizante = "",
                    Inscricao = "",
                    Cep = "02013010",
                    Endereco = "Rua Henrique Bernardelli",
                    Numero = "136",
                    Complemento = "Sala 92",
                    Bairro = "Santana",
                    Cidade = "São Paulo",
                    Uf = "SP",
                    Email = "listafacilcontrato@gmail.com",
                    Site = "http://www.listadeenderecos.info",
                    Telefones = "(11) 3256-6516 (11) 3256-2285 (11) 3256-2353 (11) 3256-0345",
                    LogoEmpresa = "listafacil.png",
                    ParametroBancoId = 1,
                    CaminhoFtp = "/filhotes/listafacil.net/www/anuncios"
                }
            };
            context.Empresas.AddOrUpdate(emps);

            var tbxas = new[]
            {
                new TipoBaixa
                {
                    TipoBaixaId = 1,
                    Descricao = "Manual",
                },
                new TipoBaixa
                {
                    TipoBaixaId = 2,
                    Descricao = "Automática",
                },
                new TipoBaixa
                {
                    TipoBaixaId = 3,
                    Descricao = "Cnab",
                },
            };
            context.TipoBaixas.AddOrUpdate(tbxas);

            context.SaveChanges();
        }
    }
}
