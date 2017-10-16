using Microsoft.AspNet.Identity;
using MvcCMS.Areas.Admin.Services;
using MvcCMS.Areas.Admin.ViewModels;
using MvcCMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MvcCMS.Areas.Admin.Controllers
{
    [RouteArea("Admin")]
    [RoutePrefix("user")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly UserService _users;

        public UserController()
        {
            _roleRepository = new RoleRepository();
            _userRepository = new UserRepository();

            _users = new UserService(ModelState, _userRepository, _roleRepository);
        }
        // GET: Admin/User

        [Route("")]
        [Route("index")]
        [Authorize(Roles ="admin")]
        public ActionResult Index()
        {

            var users = _userRepository.GetAllUsers();
            return View(users);

        }

        [HttpGet]
        [Route("create")]
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            var model = new UserViewModel();

            model.LoadUserRoles( _roleRepository.GetAllRoles());
            
            return View(model);
        }

        [HttpPost]
        [Route("create")]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserViewModel model)
        {
            var completed = await _users.CreateAsync(model);
            if (completed)
            {
                return RedirectToAction("index");
            }
            else
            {
                return View(model);
            }
      
        }

        [Route("edit/{username}")]
        [HttpGet]
        [Authorize(Roles = "admin,editor,author")]
        public async Task<ActionResult> Edit(string username)
        {
            var currentUser = User.Identity.Name;

            if(!User.IsInRole("admin") &&
                !string.Equals(currentUser, username, StringComparison.CurrentCultureIgnoreCase))
            {
                return new HttpUnauthorizedResult("You don't have the permisssions to edit details of a user other than yourself");
            }

            var user = await _users.GetUserByNameAsync(username);
            if(user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        [Route("edit/{username}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,editor,author")]
        public async Task <ActionResult> Edit(UserViewModel model,string username)
        {
            var currentUser = User.Identity.Name;
            var isAdmin =    User.IsInRole("admin");
            if (!isAdmin &&
                !string.Equals(currentUser, username, StringComparison.CurrentCultureIgnoreCase))
            {
                return new HttpUnauthorizedResult("You don't have the permisssions to edit details of a user other than yourself");
            }
            var verificationResult = await _users.UpdateUserAsync(model);
            if (verificationResult == VerificationResult.HttpNotFound)
                return HttpNotFound();
            if (verificationResult == VerificationResult.Success)
            {
                if (isAdmin)
                {
                    return RedirectToAction("index");
                }

                return RedirectToAction("index","admin");
            }

            return View(model);

        }


        [Route("delete/{username}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async  Task<ActionResult> Delete(string username)
        {
            await _users.DeleteAsync(username);
            return RedirectToAction("index");

        }

        private bool _isDisposed;
        protected override void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                _roleRepository.Dispose();
                _userRepository.Dispose();
            }
            _isDisposed = true;
                base.Dispose(disposing);

        }
    }
}