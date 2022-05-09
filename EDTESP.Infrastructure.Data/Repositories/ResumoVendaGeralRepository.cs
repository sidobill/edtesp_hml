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
    public class ResumoVendaGeralRepository : IResumoVendaGeralRepository
    {
        public List<ResumoVendaGeral> RetornarDados(int vendedor, DateTime dataInicial, DateTime dataFinal, int time = 0)
        {
            using (DbContexto contexto = new DbContexto())
            {
                var query = new StringBuilder();
                query.Append("SELECT c.Numero, cl.RazaoSocial, c.DataCriacao, c.AnoEdicao, t.Descricao AS 'Time', COUNT(1) AS 'QtdParcela', c.ValorFinal ");
                query.Append("FROM contrato c ");
                query.Append("INNER JOIN cliente cl ON cl.ClienteId = c.ClienteId ");
                query.Append("INNER JOIN vendedor v ON v.VendedorId = c.VendedorId ");
                query.Append("INNER JOIN time t ON t.TimeId = v.TimeId ");
                query.Append("INNER JOIN statuscontrato sc ON sc.StatusContratoId = c.StatusContratoId ");
                query.Append("INNER JOIN titulo ti ON ti.ContratoId = c.ContratoId ");
                query.Append("WHERE c.StatusContratoId = 1 ");

                if (vendedor > 0)
                    query.Append("AND c.VendedorId = " + vendedor + " ");

                if (time > 0)
                    query.Append("AND v.TimeId = " + time + " ");

                if (dataInicial != DateTime.MinValue && dataFinal != DateTime.MinValue)
                    query.Append("AND (c.DataCriacao BETWEEN '" + dataInicial.ToString("yyyy-MM-dd HH:mm:ss") + "' AND '" + dataFinal.ToString("yyyy-MM-dd HH:mm:ss") + "') ");

                query.Append("GROUP BY c.ContratoId");

                var resultado = contexto.Database.SqlQuery<ResumoVendaGeral>(query.ToString()).ToList();

                return resultado;
            }
        }
    }
}
