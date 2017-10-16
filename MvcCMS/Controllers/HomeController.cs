using MvcCMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MvcCMS.Controllers
{
    [RoutePrefix("")]
    public class HomeController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly int _pageSize = 2;

        public HomeController()
            :this(new PostRepository())
        { }

        public HomeController(IPostRepository postRepo)
        {
            _postRepository = postRepo;
        }

        // GET: Default

        [Route("")]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            //var posts = await _postRepository.GetPublishedPostsAsync();
            var posts = await _postRepository.GetPageAsync(1, _pageSize);
            ViewBag.PreviousPage = 0;
            ViewBag.NextPage = (Decimal.Divide(_postRepository.CountPublished, _pageSize) > 1) ? 2 : -1;
            return View(posts);

            /*

             Add MarkdownSharp in nuget packages

            var markdown = new MarkdownSharp.Markdown();

            foreach(var post  in posts){

              post.content = markdown.Transform(post.content)
            }


           */
        }

        [Route("page{page:int}")]
        public async Task<ActionResult> Page(int page = 1)
        {
            if (page < 2) return RedirectToAction("index");

            var posts = await _postRepository.GetPageAsync(page, _pageSize);
            ViewBag.PreviousPage = page - 1 ;
            ViewBag.NextPage = (Decimal.Divide(_postRepository.CountPublished, _pageSize) > page) ? page + 1  : -1;

            return View("index",posts);
         
        }

        [Route("posts/{postId}")]
        public ActionResult Post(string postId)
        {
            var post =  _postRepository.Get(postId);
            if(post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        [Route("tags/{tagId}")]
        public async  Task<ActionResult> Tag(string tagId)
        {
            var posts = await _postRepository.GetPostsByTagIdAsync(tagId);
            if (!posts.Any())
            {
                return HttpNotFound();
            }
            ViewBag.Tag = tagId;
            return View(posts);
        }
        
        [Route("contact")]
        public ActionResult Contact()
        {
            return View();
        }

    }
}