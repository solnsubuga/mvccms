using Microsoft.Owin.Security;
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
    [RouteArea("admin")]
    [Authorize]
    public class AdminController : Controller
    {
        // GET: Admin/Admin
        private readonly IUserRepository _users;
        private readonly IPostRepository _posts;
        private readonly ITagRepository _tags;
        public AdminController():
            this(new UserRepository(),new PostRepository(),new TagRepository())
        { }
        public AdminController(IUserRepository userRepository,IPostRepository postRepo,ITagRepository tagRepo)
        {
            this._users = userRepository;
            _posts = postRepo;
            _tags = tagRepo;
        }


        [Route("")]
        [Route("dashboard")]
        public async Task<ActionResult> Index()
        {
            var userCount = _users.GetAllUsers().Count();
            ViewBag.UserCount = userCount;

            var posts = await _posts.GetAllAsync();
            ViewBag.PostCount = posts.Count();

            var tagsCount = _tags.GetAll().Count();
            ViewBag.TagCount = tagsCount;
            return View();
        }

        [HttpGet]
        [Route("login")]
        [AllowAnonymous]
        public  ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _users.GetLoginUserAsync(model.UserName, model.Password);
            if(user == null)
            {
                ModelState.AddModelError(string.Empty, "Wrong username or password");
            }

            var authManager = HttpContext.GetOwinContext().Authentication;
            var userIdentity = await _users.CreateIdentityAsync(user);

            authManager.SignIn(
                new AuthenticationProperties() { IsPersistent = model.RememberMe},userIdentity);
            return RedirectToAction("index");
        }

        [Route("logout")]
        public ActionResult Logout()
        {
            var authManager = HttpContext.GetOwinContext().Authentication;
            authManager.SignOut();

            return RedirectToAction("index", new { controller = "home"});
        }

        [AllowAnonymous]
        public async Task<PartialViewResult> AdminMenu()
        {
            var items = new List<AdminMenuItem>();
            if (User.Identity.IsAuthenticated)
            {
                items.Add(new AdminMenuItem
                {
                    Text="Dashboard",
                    Action = "index",
                    RouteInfo = new {controller = "admin", area="admin"}
                });

                if (User.IsInRole("admin"))
                {
                    items.Add(new AdminMenuItem
                    {
                        Text="Users",
                        Action= "index",
                        RouteInfo = new {controller="user",area="admin"}
                    });
                }
                else
                {
                    items.Add(new AdminMenuItem
                    {
                        Text = "Profile",
                        Action = "edit",
                        RouteInfo = new { controller = "user", area = "admin" ,username= User.Identity.Name}
                    });
                }

                if (!User.IsInRole("author"))
                {
                    items.Add(new AdminMenuItem
                    {
                        Text = "Tags",
                        Action = "index",
                        RouteInfo = new { controller = "tag", area = "admin" }
                    });
                }
                items.Add(new AdminMenuItem
                {
                    Text = "Posts",
                    Action = "index",
                    RouteInfo = new { controller = "post", area = "admin" }
                });
            }

            return PartialView(items);
        }

        [AllowAnonymous]
        public PartialViewResult AdminArticleMenu()
        {
            var items = new List<AdminMenuItem>();

                items.Add(new AdminMenuItem
                {
                    Text = "Create New",
                    Action = "create",
                    RouteInfo = new { controller = "post", area = "admin" }
                });
            items.Add(new AdminMenuItem
            {
                Text = "View Articles",
                Action = "index",
                RouteInfo = new { controller = "post", area = "admin" }
            });


            return PartialView(items);
        }

        [AllowAnonymous]
        public  PartialViewResult AuthenticationLink()
        {
            var item = new AdminMenuItem
            {
                RouteInfo = new { controller = "admin", area = "admin" }
            };

            if (User.Identity.IsAuthenticated)
            {
                item.Text = "Logout";
                item.Action = "logout";
            }
            else{
                item.Text = "Log in";
                item.Action = "login";
            }

            return PartialView("_menuLink",item);
        }

        private bool _isDisposed;
        protected override void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                _users.Dispose();
            }
            _isDisposed = true;
            base.Dispose(disposing);

        }
    }
}