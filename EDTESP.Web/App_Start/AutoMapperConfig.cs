using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using EDTESP.Web.Mappers;

namespace EDTESP.Web
{
    public class AutoMapperConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<DomainToViewProfile>();
                cfg.AddProfile<ViewToDomainProfile>();
            });
        }
    }
}