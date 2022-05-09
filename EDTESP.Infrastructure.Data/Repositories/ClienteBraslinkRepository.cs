using EDTESP.Domain.Entities;
using EDTESP.Domain.Repositories;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace EDTESP.Infrastructure.Data.Repositories
{
    public class ClienteBraslinkRepository : IClienteBraslinkRepository
    {
        private const string _conexao = "server=saopaulo.braslink.com;port=3306;database=edtespco_painel;uid=edtespco_painel;password=TY_g,^N&zOrI";

        public bool ExcluirClienteBraslink(int clienteId)
        {
            using (var conexao = new MySqlConnection(_conexao))
            {
                var query = new StringBuilder();
                query.Append("DELETE FROM clientes WHERE codigoEdtesp = @codigoEdtesp");

                using (var comando = new MySqlCommand(query.ToString(), conexao))
                {
                    comando.Parameters.AddWithValue("@codigoEdtesp", clienteId);

                    conexao.Open();

                    var resultado = comando.ExecuteNonQuery();

                    if (resultado == 1)
                        return true;
                    else
                        return false;
                }
            }
        }

        public bool Inserir(ClienteBraslink clienteBraslink)
        {
            using (var conexao = new MySqlConnection(_conexao))
            {
                var query = new StringBuilder();
                query.Append("INSERT INTO clientes(tipo, letra, cliente, log, logradouro, numero, compl, bairro, cep, cidade, uf, ddd, telefone, email, logotipo, codigoEdtesp, site) ");
                query.Append("VALUES(@tipo, @letra, @cliente, @log, @logradouro, @numero, @compl, @bairro, @cep, @cidade, @uf, @ddd, @telefone, @email, @logotipo, @codigoEdtesp, @site)");

                using (var comando = new MySqlCommand(query.ToString(), conexao))
                {
                    comando.Parameters.AddWithValue("@tipo", clienteBraslink.Tipo);
                    comando.Parameters.AddWithValue("@letra", clienteBraslink.Letra);
                    comando.Parameters.AddWithValue("@cliente", clienteBraslink.Cliente);
                    comando.Parameters.AddWithValue("@log", clienteBraslink.Log);
                    comando.Parameters.AddWithValue("@logradouro", clienteBraslink.Logradouro);
                    comando.Parameters.AddWithValue("@numero", clienteBraslink.Numero);
                    comando.Parameters.AddWithValue("@compl", clienteBraslink.Compl);
                    comando.Parameters.AddWithValue("@bairro", clienteBraslink.Bairro);
                    comando.Parameters.AddWithValue("@cep", clienteBraslink.Cep);
                    comando.Parameters.AddWithValue("@cidade", clienteBraslink.Cidade);
                    comando.Parameters.AddWithValue("@uf", clienteBraslink.Uf);
                    comando.Parameters.AddWithValue("@ddd", clienteBraslink.Ddd);
                    comando.Parameters.AddWithValue("@telefone", clienteBraslink.Telefone);
                    comando.Parameters.AddWithValue("@email", clienteBraslink.Email);
                    comando.Parameters.AddWithValue("@logotipo", clienteBraslink.Logotipo);
                    comando.Parameters.AddWithValue("@codigoEdtesp", clienteBraslink.CodigoEdtesp);
                    comando.Parameters.AddWithValue("@site", clienteBraslink.WebSite);

                    conexao.Open();

                    var resultado = comando.ExecuteNonQuery();

                    if (resultado == 1)
                        return true;
                    else
                        return false;
                }
            }
        }

        public List<int> RetornarClientesIntegrados()
        {
            var resultado = new List<int>();
            using(var conexao = new MySqlConnection(_conexao))
            {
                var query = new StringBuilder();
                query.Append("SELECT * FROM clientes WHERE codigoEdtesp IS NOT NULL AND codigoEdtesp > 0");

                using (var comando = new MySqlCommand(query.ToString(), conexao))
                {
                    conexao.Open();

                    using (var reader = comando.ExecuteReader())
                    {
                        if(reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                resultado.Add(Convert.ToInt32(reader["codigoEdtesp"]));
                            }
                        }

                        return resultado;
                    }
                }
            }
        }

        public bool Atualizar(ClienteBraslink clienteBraslink)
        {
            using (var conexao = new MySqlConnection(_conexao))
            {
                var query = new StringBuilder();
                query.Append("UPDATE clientes SET cliente = @cliente, logradouro = @logradouro, numero = @numero, compl = @numero, bairro = @bairro, cep = @cep, ");
                query.Append("cidade = @cidade, uf = @uf, telefone = @telefone, email = @email, site = @site, logotipo = @logotipo WHERE  codigoEdtesp = @codigoEdtesp ");

                using (var comando = new MySqlCommand(query.ToString(), conexao))
                {
                    comando.Parameters.AddWithValue("@cliente", clienteBraslink.Cliente);
                    comando.Parameters.AddWithValue("@logradouro", clienteBraslink.Logradouro);
                    comando.Parameters.AddWithValue("@numero", clienteBraslink.Numero);
                    comando.Parameters.AddWithValue("@compl", clienteBraslink.Compl);
                    comando.Parameters.AddWithValue("@bairro", clienteBraslink.Bairro);
                    comando.Parameters.AddWithValue("@cep", clienteBraslink.Cep);
                    comando.Parameters.AddWithValue("@cidade", clienteBraslink.Cidade);
                    comando.Parameters.AddWithValue("@uf", clienteBraslink.Uf);
                    comando.Parameters.AddWithValue("@telefone", clienteBraslink.Telefone);
                    comando.Parameters.AddWithValue("@email", clienteBraslink.Email);
                    comando.Parameters.AddWithValue("@codigoEdtesp", clienteBraslink.CodigoEdtesp);
                    comando.Parameters.AddWithValue("@site", clienteBraslink.WebSite);
                    comando.Parameters.AddWithValue("@logotipo", clienteBraslink.Logotipo);

                    conexao.Open();

                    var resultado = comando.ExecuteNonQuery();

                    if (resultado == 1)
                        return true;
                    else
                        return false;
                }
            }
        }

        public ClienteBraslink RetornarPeloClienteId(int clienteId)
        {
            var resultado = new ClienteBraslink();

            using (var conexao = new MySqlConnection(_conexao))
            {
                var query = new StringBuilder();
                query.Append("SELECT * FROM  clientes WHERE codigoEdtesp = @codigoEdtesp ");

                using (var comando = new MySqlCommand(query.ToString(), conexao))
                {
                    comando.Parameters.AddWithValue("@codigoEdtesp", clienteId);

                    conexao.Open();

                    using (var reader = comando.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                resultado.Cliente = reader["cliente"].ToString();
                                resultado.Logradouro = reader["logradouro"].ToString();
                                resultado.Numero = reader["numero"].ToString();
                                resultado.Compl = reader["compl"].ToString();
                                resultado.Bairro = reader["bairro"].ToString();
                                resultado.Cep = reader["cep"].ToString();
                                resultado.Cidade = reader["cidade"].ToString();
                                resultado.Uf = reader["uf"].ToString();
                                resultado.Telefone = reader["telefone"].ToString();
                                resultado.Email = reader["email"].ToString();
                                resultado.WebSite = reader["site"].ToString();
                                resultado.Logotipo = reader["logotipo"].ToString();
                            }
                        }
                    }

                    return resultado;
                }
            }
        }
    }
}
