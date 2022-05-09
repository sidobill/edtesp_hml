using EDTESP.Application.Interfaces;
using EDTESP.Domain.Entities;
using EDTESP.Domain.Entities.Relatorios;
using EDTESP.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTESP.Application
{
    public class ComissaoIndividualService : IComissaoInvidualAppService
    {
        private readonly IComissaoIndividualRepository _comissaoIndividualRepository;
        private readonly IAppServiceBase<Vendedor> _appServiceBaseVendedor;

        public ComissaoIndividualService(IComissaoIndividualRepository comissaoIndividualRepository, IAppServiceBase<Vendedor> appServiceBaseVendedor)
        {
            _comissaoIndividualRepository = comissaoIndividualRepository;
            _appServiceBaseVendedor = appServiceBaseVendedor;
        }

        public List<ComissaoIndividual> RetornarDados(int vendedorId, DateTime dataInicial, DateTime dataFinal)
        {
            var timeId = 0;
            var percentualComissao = 0.0M;

            if(vendedorId > 0)
            {
                var vendedor = _appServiceBaseVendedor.Get(vendedorId);

                if (vendedor != null && vendedor.VendedorId > 0 && vendedor.CargoId != 4)
                {
                    timeId = vendedor.Time.TimeId;
                    vendedorId = 0;
                    percentualComissao = vendedor.Comissao;
                }
            }

            var resultado = _comissaoIndividualRepository.RetonarDados(vendedorId, dataInicial, dataFinal, timeId);

            if (resultado != null && resultado.Any())
            {
                if (percentualComissao > 0)
                {
                    foreach (var item in resultado)
                    {
                        item.ValorReceber = item.ValorFinal * (Convert.ToDouble(percentualComissao) / 100);
                        item.Comissao = Convert.ToDouble(percentualComissao);
                    }
                }
                else
                {
                    foreach (var item in resultado)
                    {
                        item.ValorReceber = item.ValorFinal * (item.Comissao / 100);
                    }
                }
            }

            return resultado;
        }
    }
}
