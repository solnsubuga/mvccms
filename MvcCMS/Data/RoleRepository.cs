using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcCMS.Data
{
   public class RoleRepository: IRoleRepository
    {
        private readonly RoleStore<IdentityRole> _store;
        private readonly RoleManager<IdentityRole> _manager;
        private readonly CmsContext _context;


        public RoleRepository()
        {
            _context = new CmsContext();
            _store = new RoleStore<IdentityRole>(_context);
            _manager = new RoleManager<IdentityRole>(_store);
        }

        public async Task<IdentityRole> GetRoleByNameAsync(string name)
        {
            var role = await _store.FindByNameAsync(name);
            return role;
        }

        public IEnumerable<IdentityRole> GetAllRoles()
        {
            return _store.Roles.ToArray();
        }

        public async Task CreateAsync(IdentityRole role)
        {
            await _manager.CreateAsync(role);
        
        }

        private bool _disposed = false;
        public void Dispose()
        {
            if(!_disposed)
            {
                _store.Dispose();
                _manager.Dispose();
            }

            _disposed = true;
        }
    }
}
