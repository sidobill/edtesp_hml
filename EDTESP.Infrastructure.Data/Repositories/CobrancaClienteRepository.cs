using EDTESP.Domain.Entities.Relatorios;
using EDTESP.Domain.Repositories;
using EDTESP.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDTESP.Infrastructure.Data.Repositories
{
    public class CobrancaClienteRepository : ICobrancaClienteRepository
    {
        public List<CobrancaCliente> RetornarDados(int cliente, DateTime dataInicial, DateTime dataFinal, DateTime dataComparacao, int vendedorId, int numeroContrato = 0)
        {
            using (DbContexto contexto = new DbContexto())
            {
                var query = new StringBuilder();
                query.Append("SELECT c.Numero, v.Nome AS 'Vendedor', cl.RazaoSocial, ti.Parcela, ti.DataVenctoReal, cl.Documento, cl.CepCobr, cl.EnderecoCobr, ");
                query.Append("ti.Valor AS 'ValorParcela', cl.BairroCobr, cl.CidadeCobr, cl.Responsavel, cl.Telefone, (SELECT COUNT(1) from titulo tt WHERE tt.ContratoId = c.ContratoId AND Removido = 0) AS 'QuantidadeTotalParcela', cl.Numero AS 'NumeroCobr', cl.ClienteId, em.Fantasia ");
                query.Append("FROM contrato c ");
                query.Append("INNER JOIN cliente cl ON cl.ClienteId = c.ClienteId ");
                query.Append("INNER JOIN vendedor v ON v.VendedorId = c.VendedorId ");
                query.Append("INNER JOIN titulo ti ON ti.ContratoId = c.ContratoId ");
                query.Append("INNER JOIN formapagamento f ON f.FormaPagamentoId = ti.FormaPagamentoId ");
                query.Append("LEFT JOIN empresa em ON em.EmpresaId = c.EmpresaId ");
                query.Append("WHERE c.StatusContratoId = 1 ");
                query.Append("AND ti.Removido = 0 AND Saldo > 0 AND ti.Suspenso = 0 ");

                if (cliente > 0)
                    query.Append("AND c.ClienteId = " + cliente + " ");

                if (vendedorId > 0)
                    query.Append("AND c.VendedorId = " + vendedorId + " ");

                if(numeroContrato > 0)
                    query.Append("AND c.Numero = " + numeroContrato + " ");

                if (dataInicial != DateTime.MinValue && dataFinal != DateTime.MinValue)
                    query.Append("AND (ti.DataVenctoReal BETWEEN '" + dataInicial.ToString("yyyy-MM-dd HH:mm:ss") + "' AND '" + dataFinal.ToString("yyyy-MM-dd HH:mm:ss") + "') ");

                query.Append("ORDER BY cl.RazaoSocial");

                var resultado = contexto.Database.SqlQuery<CobrancaCliente>(query.ToString()).ToList();

                return resultado;
            }
        }
    }
}
