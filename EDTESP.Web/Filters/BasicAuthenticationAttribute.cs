using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Filters;
using EDTESP.Infrastructure.CC.Util;

namespace EDTESP.Web.Filters
{
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            else
            {
                // Gets header parameters  
                var authenticationString = actionContext.Request.Headers.Authorization.Parameter;
                var originalString = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationString));

                // Gets username and password  
                var user = originalString.Split(':')[0];
                var pass = originalString.Split(':')[1];

                // Validate username and password  
                if (user != EdtespConfig.ApiUser || pass != EdtespConfig.ApiPass)
                {
                    // returns unauthorized error  
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
            }

            base.OnAuthorization(actionContext);
        }
    }
}