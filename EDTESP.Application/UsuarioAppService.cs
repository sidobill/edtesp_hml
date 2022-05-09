using System;
using System.Collections.Generic;
using System.Linq;
using EDTESP.Application.Interfaces;
using EDTESP.Domain.Entities;
using EDTESP.Domain.Repositories;
using EDTESP.Infrastructure.CC.Util;

namespace EDTESP.Application
{
    public class UsuarioAppService : AppServiceBase<Usuario>, IUsuarioAppService
    {
        private const string ChaveCrpt = "EDT3SP!2018";
        private IRepositoryBase<PermissaoUsuario> _repoPrmUsr;
        private IRepositoryBase<PermissaoGrupoUsuario> _repoPrmGrpUsr;
        private IRepositoryBase<Permissao> _repoPrm;

        public UsuarioAppService(IUnitOfWork unitOfWork,
            IRepositoryBase<Usuario> repoBase,
            IRepositoryBase<Permissao> repoPrm,
            IRepositoryBase<PermissaoUsuario> repoPrmUsr,
            IRepositoryBase<PermissaoGrupoUsuario> repoPrmGrpUsr) : base(unitOfWork, repoBase)
        {
            _repoPrm = repoPrm;
            _repoPrmUsr = repoPrmUsr;
            _repoPrmGrpUsr = repoPrmGrpUsr;
        }

        public Usuario Autenticar(string login, string senha)
        {
            var usr = RepoBase.Where(x => x.Login == login && !x.Removido).FirstOrDefault();

            if(usr == null)
                throw new Exception("Usuário não existe");

            var hpwd = senha.CryptData(ChaveCrpt);

            if(usr.Senha != hpwd)
                throw new Exception("Senha incorreta");

            if(usr.StatusId == (int)EnumStatus.Inativo)
                throw new Exception("Usuário inativo");

            //usr.Senha = null;
            return usr;
        }

        public IEnumerable<Permissao> ObterPermissoes(int usuarioId)
        {
            var usr = Get(usuarioId);

            if(usr == null)
                return new List<Permissao>();

            var prmsusr = _repoPrmUsr.Where(x => x.UsuarioId == usr.UsuarioId).Select(x => x.PermissaoId).ToList();
            var prmsgrp = _repoPrmGrpUsr.Where(x => x.GrupoUsuarioId == usr.GrupoUsuarioId).Select(x => x.PermissaoId).ToList();

            var prms = prmsusr.Union(prmsgrp).ToList();

            return _repoPrm.Where(x => !x.Removido && prms.Contains(x.PermissaoId)).ToList();
        }

        public new void Insert(Usuario obj)
        {
            obj.Senha = obj.Senha.CryptData(ChaveCrpt);
            base.Insert(obj);
        }
    }
}