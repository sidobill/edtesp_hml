using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using EDTESP.Domain.Entities;
using EDTESP.Infrastructure.CC.Util;
using Gerencianet.SDK;

namespace EDTESP.Infrastructure.CC.GNet
{
    public class GNetHelper
    {
        public struct StatusCharge
        {
            public const string NaoPago = "unpaid";
            public const string Pago = "paid";
            public const string MarcadoPago = "settled";
        }
        
        private static dynamic CreateEndopoint(string chave, string secret)
        {
            return new Endpoints(chave, secret, EdtespConfig.GNetSandbox);
        }

        private static dynamic GerarCharge(dynamic ep, string customId, decimal value, string description)
        {
            var vlr = (int)(value * 100);

            var chr = new
            {
                items = new[]
                {
                    new
                    {
                        name = description,
                        value = vlr,
                        amount = 1
                    }
                },
                metadata = new
                {
                    custom_id = customId
                }
            };

            return ep.CreateCharge(null, chr);
        }

        public static KeyValuePair<string, string> EmitirBoleto(string chave, string secret, string customId, decimal value, string description, DateTime vencto, decimal juros, decimal multa, string cliente,
            string tipoPessoa, string documento, string telefone, string email, string message)
        {
            var ep = CreateEndopoint(chave, secret);

            var charge = GerarCharge(ep, customId, value, description);

            dynamic bol;
            if (tipoPessoa == "J")
            {
                bol = new
                {
                    payment = new
                    {
                        banking_billet = new
                        {
                            expire_at = vencto.ToString("yyyy-MM-dd"),
                            customer = new
                            {
                                //email = email,
                                phone_number = telefone,
                                juridical_person = new
                                {
                                    corporate_name = cliente,
                                    cnpj = documento
                                }
                            }
                        }
                    }
                };
            }
            else
            {
                bol = new
                {
                    payment = new
                    {
                        banking_billet = new
                        {
                            expire_at = vencto.ToString("yyyy-MM-dd"),
                            customer = new
                            {
                                name =cliente,
                                cpf = documento,
                                //email = email,
                                phone_number = telefone,
                            },
                            configurations = new
                            {
                                fine = multa.ToString(CultureInfo.CurrentCulture).ClearNumber(),
                                interest = juros.ToString(CultureInfo.CurrentCulture).ClearNumber()
                            },
                            message = message
                        }
                    }
                };
            }

            var prms = new
            {
                id = charge.data.charge_id
            };

            var resp = ep.PayCharge(prms, bol);

            if ((int)resp.code != 200)
                throw new Exception("Erro ao gerar o Boleto junto a GerenciaNet");
            
            var chargeId = (string) resp.data.charge_id;
            var codbar = ((string) resp.data.barcode).ClearNumber();
            
            return new KeyValuePair<string, string>(chargeId, codbar);
        }

        public static KeyValuePair<int, byte[]> BaixarBoleto(string chave, string secret, int chargeId)
        {
            var ep = CreateEndopoint(chave, secret);

            var prm = new
            {
                id = chargeId
            };

            var resp = ep.DetailCharge(prm);

            if(resp.code != 200)
                throw new Exception(resp.error_description.message);

            var link = (string)resp.data.payment.banking_billet.pdf.charge;

            var wb = new WebClient();
            var data = wb.DownloadData(link);

            return new KeyValuePair<int, byte[]>(chargeId, data);
        }

        public static void CancelarBoleto(string chave, string secret, string chargeId)
        {
            var ep = CreateEndopoint(chave, secret);

            var prm = new
            {
                id = Convert.ToInt32(chargeId)
            };

            var resp = ep.CancelCharge(prm);

            if (resp.code != 200)
                throw new Exception(resp.error_description.message);
        }

        public static void MarcarBoletoPago(string chave, string secret, string chargeId)
        {
            var ep = CreateEndopoint(chave, secret);

            var prm = new 
            {
                id = Convert.ToInt32(chargeId)
            };

            var resp = ep.SettleCharge(prm);

            if (resp.code != 200)
                throw new Exception(resp.error_description.message);
        }

        public static void GerarRemessa(Empresa emp, List<Boleto> bols, string folderpath, string filename, int seq)
        {
            var path = Path.Combine(folderpath, filename);
            using (var file = new FileStream(path, FileMode.OpenOrCreate))
            {
                using (var sw = new StreamWriter(file))
                {
                    sw.WriteLine($"{emp.EmpresaId.ToString().PadLeft(9,'0')} {emp.RazaoSocial.ToUpper().MaxLength(leadWith:true)} GNET REMESSA {seq.ToString().PadLeft(9,'0')}");

                    var i = 1;
                    foreach (var boleto in bols)
                    {
                        sw.Write($"{(++i).ToString().PadLeft(6,'0')} ");
                        sw.Write($" {boleto.NossoNumeroBanco.MaxLength(10,true,'0')} {boleto.Valor.ToStrValue(leadWithChar:true, countChar:15)}");
                        sw.Write($" {boleto.Cliente.RazaoSocial.ToUpper().MaxLength(leadWith:true)}");
                        sw.Write($" {boleto.DataVenctoReal:yyyyMMdd}");
                        sw.WriteLine();
                    }
                }
            }
        }

        public static void VerificarPgto(Boleto bol, string chave, string secret)
        {
            var ep = CreateEndopoint(chave, secret);

            var prm = new
            {
                id = Convert.ToInt32(bol.NossoNumeroBanco)
            };

            var resp = ep.DetailCharge(prm);

            if(resp.code != 200)
                throw new Exception(resp.error_description.message);

            var foipago = new[] {StatusCharge.Pago, StatusCharge.MarcadoPago}.Contains((string) resp.data.status);

            if(!foipago)
                return;

            var pgto = ((IEnumerable<dynamic>) resp.data.history)
                .FirstOrDefault(x => ((string) x.message).Contains("Pagamento de cobrança"));

            if(pgto == null)
                return;

            var dtpgto = (DateTime) pgto.created_at;
            var vlrpago = ((string) resp.data.paid_value).StrNumToDecimal();

            bol.Saldo = bol.Valor - vlrpago;
            if (bol.Saldo < 0)
                bol.Saldo = 0;

            bol.DataPgto = dtpgto;
            bol.DataDisponib = dtpgto;
        }

    }
}