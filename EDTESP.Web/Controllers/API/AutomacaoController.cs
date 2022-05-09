using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;
using EDTESP.Application.Interfaces;
using EDTESP.Infrastructure.CC.Util;
using EDTESP.Web.Filters;

namespace EDTESP.Web.Controllers.API
{
    [BasicAuthentication]
    public class AutomacaoController : ApiController
    {
        private readonly ITituloAppService _appsvcTit;

        public AutomacaoController(ITituloAppService appsvcTit)
        {
            _appsvcTit = appsvcTit;
        }

        [HttpGet]
        [Route("api/boletos/gerar")]
        public IHttpActionResult GerarBoletos()
        {
            try
            {
                var path = HttpContext.Current.Server.MapPath(EdtespConfig.AppDataFolder);
                _appsvcTit.BaixarboletosNaoGerados(path);
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpGet]
        [Route("api/boletos/enviar")]
        public IHttpActionResult EnviarBoletos()
        {
            try
            {
                var path = HttpContext.Current.Server.MapPath(EdtespConfig.AppDataFolder);
                //_appsvcTit.EnviarBoletosAoCliente(path);
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpGet]
        [Route("api/boletos/verificar")]
        public IHttpActionResult VerificarBoletosGNet()
        {
            try
            {
                _appsvcTit.ProcessarBoletosGnet();
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

    }
}
