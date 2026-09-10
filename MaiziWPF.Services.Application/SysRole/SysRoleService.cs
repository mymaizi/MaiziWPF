using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using System.Collections.Generic;

namespace MaiziWPF.Services.Application
{
    public class SysRoleService : ISysRoleService
    {
        private readonly ISysRoleRepository _repository;

        public SysRoleService(ISysRoleRepository repository)
        {
            _repository = repository;
        }

        public List<SysRole> SelectRoleList(QueryRoleInput input)
        {
            return _repository.SelectRoleList(input);
        }

        public SysRole SelectRoleById(long roleId)
        {
            return _repository.SelectRoleById(roleId);
        }

        public int InsertRole(SysRole role)
        {
            return _repository.InsertRole(role);
        }

        public int UpdateRole(SysRole role)
        {
            return _repository.UpdateRole(role);
        }

        public int DeleteRoleById(long roleId)
        {
            return _repository.DeleteRoleById(roleId);
        }

        public bool CheckRoleNameUnique(SysRole role)
        {
            return _repository.CheckRoleNameUnique(role);
        }

        public bool CheckRoleKeyUnique(SysRole role)
        {
            return _repository.CheckRoleKeyUnique(role);
        }

        public bool CheckRoleExistUser(long roleId)
        {
            return _repository.CheckRoleExistUser(roleId);
        }
    }
}