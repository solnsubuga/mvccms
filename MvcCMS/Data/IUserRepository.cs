using MvcCMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MvcCMS.Data
{
   public interface IUserRepository: IDisposable
    {
        Task<CmsUser> GetUserByNameAsync(string username);
        IEnumerable<CmsUser> GetAllUsers();
        Task CreateAsync(CmsUser user, string password);
        Task DeleteAsync(CmsUser user);
        Task UpdateAsync(CmsUser user);
        bool VerifyUserPassword(string passwordHash, string currentPassword);
        string HashPassword(string newPassword);
        Task AddUserToRoleAsync(CmsUser user, string selectedRole);
        Task<IEnumerable<string>> GetRolesForUserAsync(CmsUser user);
        Task RemoveUserFromRoleAsync(CmsUser user, params string [] roleNames);
        Task<CmsUser> GetLoginUserAsync(string userName, string password);
        Task<ClaimsIdentity> CreateIdentityAsync(CmsUser user);
    }
}
