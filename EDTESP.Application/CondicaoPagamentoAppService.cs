using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using EDTESP.Application.Interfaces;
using EDTESP.Domain.Entities;
using EDTESP.Domain.Repositories;

namespace EDTESP.Application
{
    public class CondicaoPagamentoAppService : AppServiceBase<CondicaoPagamento>, ICondicaoPagamentoAppService
    {
        private readonly ICondicaoPagamentoRepository _appRepo;
        private readonly IRepositoryBase<Feriado> _repoFer;
        public const string RgxModelo1 = @"([0-9]+)\/([0-9]+)";
        public const string RgxModelo2 = @"d([0-9]+)\/([0-9]+)";
        public const string RgxModelo3 = @"([0-9]+),{0,1}";

        public CondicaoPagamentoAppService(IUnitOfWork unitOfwork,
            IRepositoryBase<CondicaoPagamento> repoBase,
            ICondicaoPagamentoRepository appRepo,
            IRepositoryBase<Feriado> repoFer) : base(unitOfwork, repoBase)
        {
            _appRepo = appRepo;
            _repoFer = repoFer;
        }

        private IEnumerable<Titulo> GerarParcelas(CondicaoPagamento cond, int formaPgtoId, DateTime database, decimal primeiraParcela, decimal valorTotal)
        {
            var lst = new List<Titulo>();
            if (Regex.IsMatch(cond.Modelo, RgxModelo1, RegexOptions.CultureInvariant))
            {
                var mth = Regex.Matches(cond.Modelo, RgxModelo1);
                var parc = Convert.ToInt32(mth[0].Groups[1].Value);
                var dias = Convert.ToInt32(mth[0].Groups[2].Value);

                //Qtde de dias max da condição
                var maxdias = parc * dias;
                var ini = database;
                var fim = ini.AddDays(maxdias);

                var feriados = _repoFer.Where(x => x.Data >= ini && x.Data <= fim).ToList();

                for (var i = 1; i <= parc; i++)
                {
                    var data = Convert.ToDateTime(database.AddDays(i * dias).ToShortDateString() + " 00:00:00");
                    var dv = data;

                    while (feriados.Any(x => x.Data.ToShortDateString() == data.ToShortDateString()) ||
                           data.DayOfWeek == DayOfWeek.Sunday || data.DayOfWeek == DayOfWeek.Saturday)
                        data = data.AddDays(1);

                    lst.Add(new Titulo
                    {
                        DataVencto = dv,
                        DataVenctoReal = data
                    });
                }
            }
            else if (Regex.IsMatch(cond.Modelo, RgxModelo2, RegexOptions.CultureInvariant))
            {
                var mth = Regex.Matches(cond.Modelo, RgxModelo2);
                var dia = Convert.ToInt32(mth[0].Groups[1].Value);
                var parcs = Convert.ToInt32(mth[0].Groups[2].Value);
                //var dias = Convert.ToInt32(mth[0].Groups[3].Value);

                //database = database.AddMonths(1);
                if (DateTime.DaysInMonth(database.Year, database.Month) < dia)
                    dia = DateTime.DaysInMonth(database.Year, database.Month);

                var ini = new DateTime(database.Year, database.Month, dia);
                var fim = ini.AddMonths(parcs);
                var feriados = _repoFer.Where(x => x.Data >= ini && x.Data <= fim);

                for (var i = 1; i <= parcs; i++)
                {
                    var data = Convert.ToDateTime(ini.AddMonths(i).ToShortDateString() + " 00:00:00");
                    var dv = data;

                    while (feriados.Any(x => x.Data == data) ||
                           data.DayOfWeek == DayOfWeek.Sunday || data.DayOfWeek == DayOfWeek.Saturday)
                        data = data.AddDays(1);

                    lst.Add(new Titulo
                    {
                        DataVencto = dv,
                        DataVenctoReal = data
                    });
                }

            }
            //  BOLETO
            else if (Regex.IsMatch(cond.Modelo, RgxModelo3, RegexOptions.CultureInvariant))
            {
                var mth = Regex.Matches(cond.Modelo, RgxModelo3);

                var dias = new List<int>();
                foreach (Match m in mth)
                    dias.Add(Convert.ToInt32(m.Groups[m.Groups.Count - 1].Value));

                dias = dias.OrderBy(x => x).ToList();
                var ult = dias.Last();

                var ini = database;
                var fim = ini.AddDays(ult);
                var feriados = _repoFer.Where(x => x.Data >= ini && x.Data <= fim);

                foreach (var d in dias)
                {
                    var data = Convert.ToDateTime(database.AddDays(d - 30).ToShortDateString() + " 00:00:00");
                    var dv = data;

                    while (feriados.Any(x => x.Data == data) ||
                           data.DayOfWeek == DayOfWeek.Sunday || data.DayOfWeek == DayOfWeek.Saturday)
                        data = data.AddDays(1);

                    lst.Add(new Titulo
                    {
                        DataVencto = dv,
                        DataVenctoReal = data
                    });
                }
            }
            else
                throw new Exception("Modelo inválido");

            if (primeiraParcela > 0)
            {
                var parcela = 1;
                var parcelas = lst.Count - 1;
                var valorParcela = (valorTotal - primeiraParcela) / parcelas;

                foreach (var item in lst)
                {
                    item.Valor = parcela == 1 ? primeiraParcela : valorParcela;
                    item.CondicaoPagamentoId = cond.CondicaoPagamentoId;
                    item.FormaPagamentoId = formaPgtoId;
                    parcela++;
                }
            }
            else
            {

                decimal vlrprc = valorTotal / lst.Count;
                lst.ForEach(x =>
                {
                    x.Valor = vlrprc;
                    x.CondicaoPagamentoId = cond.CondicaoPagamentoId;
                    x.FormaPagamentoId = formaPgtoId;
                });
            }

            //Corrige arredondamentos 
            var somaparcs = lst.Select(x => x.Valor).Sum(x => x);
            var dif = valorTotal - somaparcs;
            var ultparc = lst.Last();
            ultparc.Valor += dif;
            return lst;
        }

        public void Insert(CondicaoPagamento obj, IEnumerable<int> formas)
        {
            _appRepo.Insert(obj, formas);
            UnitOfWork.Save();
        }

        public void Update(CondicaoPagamento obj, IEnumerable<int> formas)
        {
            _appRepo.Update(obj,formas);
            UnitOfWork.Save();
        }

        public void Delete(int id)
        {
            var obj = _appRepo.Find(id);
            obj.Removido = true;
            _appRepo.Update(obj);
            UnitOfWork.Save();
        }

        public IQueryable<FormaPagamento> WhereFormasPgto(Expression<Func<FormaPagamento, bool>> expression)
        {
            return _appRepo.WhereFormasPgto(expression);
        }

        public IEnumerable<Titulo> GerarParcelas(int condicaoId, int formaId, int cond1ParcId, int forma1ParcId, DateTime database, decimal valorTotal,  decimal primparc = 0)
        {
            var cond = _appRepo.Where(x => !x.Removido && x.CondicaoPagamentoId == condicaoId).FirstOrDefault();
            var cond1Parc = _appRepo.Where(x => !x.Removido && x.CondicaoPagamentoId == cond1ParcId).FirstOrDefault();

            if (cond == null || cond1Parc == null)
                throw new Exception("Condição de pgto não encontrada");
            
            var parcs = new List<Titulo>();

            //var vlrdemais = valorTotal;

            //if (primparc > 0)
            //{
            //    parcs.AddRange(GerarParcelas(cond1Parc, forma1ParcId, database, primparc));

            //    vlrdemais = vlrdemais - primparc;
            //}

            parcs.AddRange(GerarParcelas(cond, formaId, database, primparc, valorTotal));

            for (var i = 0; i < parcs.Count; i++)
            {
                parcs[i].Parcela = i + 1;
            }

            return parcs;
        }

        public bool ValidarModelo(string modelo)
        {
            if (Regex.IsMatch(modelo, RgxModelo1, RegexOptions.CultureInvariant))
                return true;
            if (Regex.IsMatch(modelo, RgxModelo2, RegexOptions.CultureInvariant))
                return true;
            if (Regex.IsMatch(modelo, RgxModelo3, RegexOptions.CultureInvariant))
                return true;

            return false;
        }
    }
}