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
    public class ResumoVendaGeralAppService : IResumoVendaGeralAppService
    {
        private readonly IResumoVendaGeralRepository _resumoVendaGeralRepository;
        private readonly IAppServiceBase<Vendedor> _appServiceBaseVendedor;

        public ResumoVendaGeralAppService(IResumoVendaGeralRepository resumoVendaGeralRepository, IAppServiceBase<Vendedor> appServiceBaseVendedor)
        {
            _resumoVendaGeralRepository = resumoVendaGeralRepository;
            _appServiceBaseVendedor = appServiceBaseVendedor;
        }

        public List<ResumoVendaGeral> RetornarDados(int vendedor, DateTime dataInicial, DateTime dataFinal)
        {
            var timeId = 0;

            if (vendedor > 0)
            {
                var vendedorObj = _appServiceBaseVendedor.Get(vendedor);

                if (vendedorObj != null && vendedorObj.VendedorId > 0 && vendedorObj.CargoId != 4)
                {
                    timeId = vendedorObj.Time.TimeId;
                    vendedor = 0;
                }
            }

            return _resumoVendaGeralRepository.RetornarDados(vendedor, dataInicial, dataFinal, timeId);
        }
    }
}
