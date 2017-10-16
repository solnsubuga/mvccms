using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcCMS.Data
{
   public interface IRoleRepository: IDisposable
    {
        Task<IdentityRole> GetRoleByNameAsync(string name);
        IEnumerable<IdentityRole> GetAllRoles();
        Task CreateAsync(IdentityRole role);
    }
}
