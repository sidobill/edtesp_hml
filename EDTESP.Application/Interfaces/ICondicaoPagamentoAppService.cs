using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EDTESP.Domain.Entities;

namespace EDTESP.Application.Interfaces
{
    public interface ICondicaoPagamentoAppService : IAppServiceBase<CondicaoPagamento>
    {
        void Insert(CondicaoPagamento obj, IEnumerable<int> formasId);

        void Update(CondicaoPagamento obj, IEnumerable<int> formasId);

        void Delete(int id);

        IQueryable<FormaPagamento> WhereFormasPgto(Expression<Func<FormaPagamento, bool>> expression);

        IEnumerable<Titulo> GerarParcelas(int condicaoId, int formaId, int cond1ParcId, int forma1ParcId,
            DateTime database, decimal valorTotal, decimal primparc = 0);

        bool ValidarModelo(string modelo);
    }
}