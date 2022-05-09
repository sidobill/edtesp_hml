using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EDTESP.Domain.Entities;

namespace EDTESP.Domain.Repositories
{
    public interface ICondicaoPagamentoRepository : IRepositoryBase<CondicaoPagamento>
    {
        IQueryable<FormaPagamento> WhereFormasPgto(Expression<Func<FormaPagamento, bool>> expression);

        void Insert(CondicaoPagamento obj, IEnumerable<int> formasId);

        void Update(CondicaoPagamento obj, IEnumerable<int> formasId);
    }
}