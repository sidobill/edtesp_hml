using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using EDTESP.Domain.Entities;
using EDTESP.Domain.Repositories;
using EDTESP.Infrastructure.Data.Context;

namespace EDTESP.Infrastructure.Data.Repositories
{
    public class CondicaoPagamentoRepository : RepositoryBase<CondicaoPagamento>, ICondicaoPagamentoRepository
    {
        public CondicaoPagamentoRepository(IDbContexto context) : base(context)
        {
        }

        public IQueryable<FormaPagamento> WhereFormasPgto(Expression<Func<FormaPagamento, bool>> expression)
        {
            return context.FormasPagamentos.Where(expression);
        }

        public void Insert(CondicaoPagamento obj, IEnumerable<int> formas)
        {
            var frms = context.FormasPagamentos.Where(x => formas.Contains(x.FormaPagamentoId)).ToList();
            obj.FormasPagamentos = frms;
            context.CondicaoPagamentos.Add(obj);
        }

        public void Update(CondicaoPagamento obj, IEnumerable<int> formas)
        {
            //context.CondicaoPagamentos.Attach(obj);
            var nobj = context.CondicaoPagamentos.Find(obj.CondicaoPagamentoId);
            nobj.Descricao = obj.Descricao;
            nobj.Modelo = obj.Modelo;
            nobj.UsuarioAtualizadorId = obj.UsuarioAtualizadorId;
            nobj.DataAtualizacao = obj.DataAtualizacao;

            foreach (var cfp in nobj.FormasPagamentos.ToList())
            {
                if (!formas.Contains(cfp.FormaPagamentoId))
                    nobj.FormasPagamentos.Remove(cfp);
            }

            foreach (var fp in formas)
            {
                if(!nobj.FormasPagamentos.Any(x => x.FormaPagamentoId.Equals(fp)))
                    nobj.FormasPagamentos.Add(context.FormasPagamentos.Find(fp));
            }

            Update(nobj);
        }
    }
}