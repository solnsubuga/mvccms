using MvcCMS.Data;
using MvcCMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MvcCMS.Areas.Admin.Controllers
{
    [RouteArea("Admin")]
    [RoutePrefix("post")]
    [Authorize]
    public class PostController : Controller
    {

        private readonly IPostRepository _repository;
        private readonly IUserRepository _users;
        public PostController()
            :this(new PostRepository(),new UserRepository() ) { }
        public PostController(IPostRepository repository,IUserRepository userRepository)
        {
            _repository = repository;
            _users = userRepository;
        }


        // GET: Admin/Post
        [Route("")]
        [Route("index")]
        public async  Task<ActionResult> Index()
        {
            if (!User.IsInRole("author"))
            {
                var posts = await _repository.GetAllAsync();
                return View(posts);
            }

            var user = await GetLoggedInUser();
            var authorPosts = await _repository.GetPostsByAuthorAsync(user.Id);
            return View(authorPosts);
           
        }


        // /admin/create/
        [HttpGet]
        [Route("create")]
        public ActionResult Create()
        {
    
            return View(new Post());
        }

        // /admin/create/
        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public async  Task<ActionResult> Create(Post model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentUser = await GetLoggedInUser();
            //Create a post
            if (string.IsNullOrWhiteSpace(model.Id))
            {
                model.Id = model.Title;
            }
            model.Id = model.Id.MakeUrlFriendly();
            model.Tags = model.Tags.Select(tag => tag.MakeUrlFriendly()).ToList();
            model.Created = DateTime.Now;
            model.AuthorId =  currentUser.Id;
            try
            {

                await _repository.CreateAsync(model);
                return RedirectToAction("index");
            }
            catch(Exception e)
            {
                //redircet back to page with errors
                ModelState.AddModelError(string.Empty, e);
                return View(model);
            }

        }

        //  /admin/post/edit/post-to-edit
        [HttpGet]
        [Route("edit/{postId}")]
        public async  Task<ActionResult> Edit(string postId)
        {
            var post = _repository.Get(postId);
            if (post == null)
            {
                return HttpNotFound();
            }
            
            if(User.IsInRole("author"))
            {
                var user = await GetLoggedInUser();
                if(post.AuthorId != user.Id)
                {
                    return new HttpUnauthorizedResult("You cannot edit a post which you didnot author");
                }
            }
            return View(post);
        }
        //  /admin/post/edit/post-to-edit
        [HttpPost]
        [Route("edit/{postId}")]
        public async Task<ActionResult> Edit(string postId, Post model)
        {
            
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (User.IsInRole("author"))
            {
                var user = await GetLoggedInUser();
                var post = _repository.Get(postId);
                try
                {
                    if (post.AuthorId != user.Id)
                    {
                        return new HttpUnauthorizedResult("You cannot edit a post which you did not author");
                    }
                }
                catch { }
               
            }

            if (string.IsNullOrWhiteSpace(model.Id))
            {
                model.Id = model.Title;
            }
            model.Id = model.Id.MakeUrlFriendly();
            model.Tags = model.Tags.Select(tag => tag.MakeUrlFriendly()).ToList();

            try
            {
                await _repository.EditAsync(postId, model);
                return RedirectToAction("index");
            }
            catch(KeyNotFoundException )
            {
                return HttpNotFound();
            }
            catch(Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);
            }
    
        }

        //  /admin/post/delete/post-to-edit
        [HttpGet]
        [Authorize(Roles ="admin,editor")]
        [Route("delete/{postId}")]
        public ActionResult Delete(string postId)
        {
            var post = _repository.Get(postId);
            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }
        //  /admin/post/delete/post-to-edit
        [HttpPost]
        [Route("delete/{postId}")]
        [Authorize(Roles = "admin,editor")]
        public async Task<ActionResult> Delete(string postId, string bar)
        {

            try
            {
                await _repository.DeleteAsync(postId);
                return RedirectToAction("index");
            }
            catch (KeyNotFoundException)
            {
                return HttpNotFound();
            }


        }

        private async Task<CmsUser> GetLoggedInUser()
        {
            return await _users.GetUserByNameAsync(User.Identity.Name);
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