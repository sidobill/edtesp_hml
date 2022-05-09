using EDTESP.Domain.Entities.Relatorios;
using EDTESP.Domain.Repositories;
using EDTESP.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDTESP.Infrastructure.Data.Repositories
{
    public class EstornoComissaoRepository : IEstornoComissaoRepository
    {
        public List<EstornoComissao> RetornarDados(int vendedor, DateTime dataInicial, DateTime dataFinal, DateTime dataComparacaoVencimento, int time = 0)
        {
            using (DbContexto contexto = new DbContexto())
            {
                var query = new StringBuilder();
                query.Append("SELECT c.Numero, c.VendedorId, cl.RazaoSocial, ti.Parcela, ti.DataVenctoReal, f.Descricao AS 'FormaPgto', c.ValorFinal, v.Comissao, ");
                query.Append("ti.Valor AS 'ValorParcela', ");
                query.Append("DATEDIFF('" + dataComparacaoVencimento.ToString("yyyy-MM-dd") + "', ti.DataVenctoReal) AS 'DiasAtraso' ");
                query.Append("FROM contrato c ");
                query.Append("INNER JOIN cliente cl ON cl.ClienteId = c.ClienteId ");
                query.Append("INNER JOIN vendedor v ON v.VendedorId = c.VendedorId ");
                query.Append("INNER JOIN titulo ti ON ti.ContratoId = c.ContratoId ");
                query.Append("INNER JOIN formapagamento f ON f.FormaPagamentoId = ti.FormaPagamentoId ");
                query.Append("WHERE c.StatusContratoId = 1 ");
                query.Append("AND c.ContratoId IN(SELECT ContratoId FROM titulo WHERE Parcela = 1 AND DataBaixa IS NOT NULL AND Removido = 0 and Saldo = 0 AND ContratoId = c.ContratoId) ");
                query.Append("AND ti.Parcela > 1 AND ti.Removido = 0 AND Saldo > 0 AND ti.DataVenctoReal < '" + dataComparacaoVencimento.ToString("yyyy-MM-dd") + "' ");

                if (vendedor > 0)
                    query.Append("AND c.VendedorId = " + vendedor + " ");

                if (time > 0)
                    query.Append("AND v.TimeId = " + time + " ");

                if (dataInicial != DateTime.MinValue && dataFinal != DateTime.MinValue)
                    query.Append("AND (ti.DataVenctoReal BETWEEN '" + dataInicial.ToString("yyyy-MM-dd HH:mm:ss") + "' AND '" + dataFinal.ToString("yyyy-MM-dd HH:mm:ss") + "') ");

                var resultado = contexto.Database.SqlQuery<EstornoComissao>(query.ToString()).ToList();

                return resultado;
            }
        }
    }
}
