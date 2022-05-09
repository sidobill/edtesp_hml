using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EDTESP.Application.Interfaces;
using EDTESP.Domain.Entities;
using EDTESP.Domain.Repositories;

namespace EDTESP.Application
{
    public class GrupoUsuarioAppService : AppServiceBase<GrupoUsuario>, IGrupoUsuarioAppService
    {
        private IGrupoUsuarioRepository _appRepo;

        public GrupoUsuarioAppService(IUnitOfWork unitOfWork,
            IRepositoryBase<GrupoUsuario> appBase,
            IGrupoUsuarioRepository appRepo) : base(unitOfWork, appBase)
        {
            _appRepo = appRepo;
        }

        public new IEnumerable<GrupoUsuario> List()
        {
            return _appRepo.List();
        }

        public new IQueryable<GrupoUsuario> Where(Expression<Func<GrupoUsuario, bool>> expression)
        {
            return _appRepo.Where(expression);
        }
    }
}