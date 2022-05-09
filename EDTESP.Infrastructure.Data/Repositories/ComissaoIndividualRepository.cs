using EDTESP.Domain.Entities.Relatorios;
using EDTESP.Domain.Repositories;
using EDTESP.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTESP.Infrastructure.Data.Repositories
{
    public class ComissaoIndividualRepository: IComissaoIndividualRepository
    {
        public List<ComissaoIndividual> RetonarDados(int vendedorId, DateTime dataInicial, DateTime dataFinal, int time = 0)
        {
            using (DbContexto contexto = new DbContexto())
            {
                var query = new StringBuilder();
                query.Append("SELECT c.Numero, cl.RazaoSocial, c.DataCriacao, c.AnoEdicao, t.Descricao AS 'Time', COUNT(1) AS 'QtdParcela', c.ValorFinal, ");
                query.Append("c.ValorBase, v.Comissao, sc.StatusContratoId, sc.Descricao AS 'Status' ");
                query.Append("FROM contrato c ");
                query.Append("INNER JOIN cliente cl ON cl.ClienteId = c.ClienteId ");
                query.Append("INNER JOIN vendedor v ON v.VendedorId = c.VendedorId ");
                query.Append("INNER JOIN time t ON t.TimeId = v.TimeId ");
                query.Append("INNER JOIN statuscontrato sc ON sc.StatusContratoId = c.StatusContratoId ");
                query.Append("INNER JOIN titulo ti ON ti.ContratoId = c.ContratoId AND ti.Removido = 0 AND ti.Suspenso = 0 ");
                query.Append("WHERE c.StatusContratoId = 1 ");
                query.Append("AND c.ContratoId IN(SELECT ContratoId FROM titulo WHERE Parcela = 1 AND DataBaixa IS NOT NULL AND Removido = 0 and Saldo = 0 AND ContratoId = c.ContratoId ");

                if (dataInicial != DateTime.MinValue && dataFinal != DateTime.MinValue)
                    query.Append("AND (DataBaixa BETWEEN '" + dataInicial.ToString("yyyy-MM-dd HH:mm:ss") + "' AND '" + dataFinal.ToString("yyyy-MM-dd HH:mm:ss") + "') ");

                query.Append(") ");

                if (vendedorId > 0)
                    query.Append("AND c.VendedorId = " + vendedorId + " ");

                if (time > 0)
                    query.Append("AND v.TimeId = " + time + " ");

                query.Append("GROUP BY c.ContratoId");

                var resultado = contexto.Database.SqlQuery<ComissaoIndividual>(query.ToString()).ToList();

                return resultado;
            }
        }
    }
}
