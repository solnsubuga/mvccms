using System;
using MvcCMS.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MvcCMS.Areas.Admin.ViewModels;
using MvcCMS.Models;

namespace MvcCMS.Areas.Admin.Services
{
 public enum VerificationResult
    {
        Success,
        Failure,
        HttpNotFound

    }
 public  class UserService
    {
        private readonly IUserRepository _users;
        private readonly IRoleRepository _roles;
        private readonly ModelStateDictionary _modelState;
        public  UserService(ModelStateDictionary modelState, 
            IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _modelState = modelState;
            _users = userRepository;
            _roles = roleRepository;
        }

        public async Task<bool> CreateAsync(UserViewModel model)
        {
            if (!_modelState.IsValid)
            {
                return false;
            }

            var existingUser = await _users.GetUserByNameAsync(model.UserName);
            if(existingUser != null)
            {
                _modelState.AddModelError(string.Empty, "The username is already in use.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(model.NewPassword))
            {
                _modelState.AddModelError(string.Empty, "Password field cannot be empty");
                return false;
            }

            //otherwise create a user

            var newUser = new CmsUser()
            {
               DisplayName = model.DisplayName,
               UserName    = model.UserName,
               Email      = model.Email,
               
            };

            await _users.CreateAsync(newUser, model.NewPassword);

            await _users.AddUserToRoleAsync(newUser, model.SelectedRole);

            return true;
        }

        public async Task<UserViewModel> GetUserByNameAsync(string name)
        {
            var user = await _users.GetUserByNameAsync(name);

            if(user == null)
            {
                return null;
            }

            var viewModel = new UserViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                DisplayName = user.DisplayName
            };

            var userRoles = await _users.GetRolesForUserAsync(user);

            viewModel.SelectedRole = userRoles.Count() > 1 ?
                userRoles.FirstOrDefault() : userRoles.SingleOrDefault();

            viewModel.LoadUserRoles(_roles.GetAllRoles());
            return viewModel;
        }

        public async Task<VerificationResult> UpdateUserAsync(UserViewModel model)
        {
            var user = await _users.GetUserByNameAsync(model.UserName);
            if(user == null)
            {
                _modelState.AddModelError(string.Empty, "The specified user does not exist.");
                return VerificationResult.HttpNotFound;
            }

            if (!_modelState.IsValid)
            {
                return VerificationResult.Failure;
            }

            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                if (string.IsNullOrWhiteSpace(model.CurrentPassword))
                {
                    _modelState.AddModelError(string.Empty, "The current password is required");
                    return VerificationResult.Failure;
                }

                var passwordVerified = _users.VerifyUserPassword(user.PasswordHash, model.CurrentPassword);
                if (!passwordVerified)
                {
                    _modelState.AddModelError(string.Empty, "The current password does not match out records");
                    return VerificationResult.Failure;
                }

                var newHashedPassword = _users.HashPassword(model.NewPassword);
                user.PasswordHash = newHashedPassword;
            }

            user.Email = model.Email;
            user.DisplayName = model.DisplayName;

            await _users.UpdateAsync(user);

            var roles = await _users.GetRolesForUserAsync(user);

            await _users.RemoveUserFromRoleAsync(user, roles.ToArray());

            await _users.AddUserToRoleAsync(user, model.SelectedRole);

            return VerificationResult.Success;
        }

        public async Task DeleteAsync(string username)
        {
            var user = await _users.GetUserByNameAsync(username);
            if (user == null)
            {
                return;
            }
            await _users.DeleteAsync(user);
        }
    }
}
