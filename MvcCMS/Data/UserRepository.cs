using Microsoft.AspNet.Identity;
using MvcCMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace MvcCMS.Data
{
   public class UserRepository: IUserRepository
    {

        private readonly CmsUserStore _store;
        private readonly CmsUserManager _manager;

        public UserRepository()
        {
            _store = new CmsUserStore();
            _manager = new CmsUserManager(_store);
        }

        public async Task<CmsUser> GetUserByNameAsync(string username)
        {
            var user = await _store.FindByNameAsync(username);
            return user;
        }

        public IEnumerable<CmsUser> GetAllUsers()
        {
            return _store.Users.ToArray();
        }

        public async Task CreateAsync(CmsUser user, string password)
        {
            await _manager.CreateAsync(user, password);
        }

        public async Task DeleteAsync(CmsUser user)
        {
            await _manager.DeleteAsync(user);
        }

        public async  Task UpdateAsync(CmsUser user)
        {
            await _manager.UpdateAsync(user);
        }

        public bool VerifyUserPassword(string passwordHash, string currentPassword)
        {
            var passwordVerified = _manager.PasswordHasher.VerifyHashedPassword(passwordHash, currentPassword);
            if (passwordVerified != PasswordVerificationResult.Success)
            {
                return false;
            }

            return true;
        }

        public string HashPassword(string password)
        {
            var hashedPassword = _manager.PasswordHasher.HashPassword(password);
            return hashedPassword;
        }

        public async Task AddUserToRoleAsync(CmsUser user, string selectedRole)
        {
            await _manager.AddToRoleAsync(user.Id, selectedRole);
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

        public async Task<IEnumerable<string>> GetRolesForUserAsync(CmsUser user)
        {
            return await _manager.GetRolesAsync(user.Id);
        }

        public async Task RemoveUserFromRoleAsync(CmsUser user, params string[] roleNames)
        {
            await _manager.RemoveFromRolesAsync(user.Id, roleNames);
        }

        public async Task<CmsUser> GetLoginUserAsync(string userName, string password)
        {
            return await _manager.FindAsync(userName, password);
        }

        public async Task<ClaimsIdentity> CreateIdentityAsync(CmsUser user)
        {
            return await _manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
        }
    }
}
