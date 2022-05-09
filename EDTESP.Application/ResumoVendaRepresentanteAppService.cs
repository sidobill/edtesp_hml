using EDTESP.Application.Interfaces;
using EDTESP.Domain.Entities;
using EDTESP.Domain.Entities.Relatorios;
using EDTESP.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EDTESP.Application
{
    public class ResumoVendaRepresentanteAppService : IResumoVendaRepresentanteAppService
    {
        private readonly IResumoVendaRepresentanteRepository _resumoVendaRepresentanteRepository;
        private readonly IAppServiceBase<Vendedor> _appServiceBaseVendedor;

        public ResumoVendaRepresentanteAppService(IResumoVendaRepresentanteRepository resumoVendaRepresentanteRepository, 
                                                  IAppServiceBase<Vendedor> appServiceBaseVendedor)
        {
            _resumoVendaRepresentanteRepository = resumoVendaRepresentanteRepository;
            _appServiceBaseVendedor = appServiceBaseVendedor;
        }

        public List<ResumoVendaRepresentante> RetornarDados(int vendedor, DateTime dataInicial, DateTime dataFinal)
        {
            var timeId = 0;
            var percentualComissao = 0.0M;

            if (vendedor > 0)
            {
                var vendedorObj = _appServiceBaseVendedor.Get(vendedor);

                if (vendedorObj != null && vendedorObj.VendedorId > 0 && vendedorObj.CargoId != 4)
                {
                    timeId = vendedorObj.Time.TimeId;
                    vendedor = 0;
                    percentualComissao = vendedorObj.Comissao;
                }
            }

            var resultado = _resumoVendaRepresentanteRepository.RetornarDados(vendedor, dataInicial, dataFinal, timeId);

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
