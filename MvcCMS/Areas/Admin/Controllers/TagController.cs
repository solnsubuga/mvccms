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
    [RoutePrefix("tag")]
    [Authorize]
    public class TagController : Controller
    {
        private readonly ITagRepository _responsitory;


        public TagController():this (new TagRepository()) {    }


        //Inject dependecy on the constructor
        public TagController(ITagRepository repository)
        {
            this._responsitory = repository;
        }

        // GET: Admin/Tag
        [Route("")]
        [Route("index")]
        public ActionResult Index()
        {
            //Get All the tags
            var tags = _responsitory.GetAll(); 

            if(Request.AcceptTypes.Contains("applicaton/json"))
            {
                return Json(tags, JsonRequestBehavior.AllowGet);
            }

            if (User.IsInRole("author"))
            {
                return new HttpUnauthorizedResult();
            }
            return View(model:tags);
        }

        [HttpGet]
        [Route("edit/{tag}")]
        [Authorize(Roles = "admin,editor")]
        public ActionResult Edit(string tag)
        {
            try
            {
                var model = _responsitory.Get(tag);
                return View(model: model);
            }
            catch(KeyNotFoundException)
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [Route("edit/{tag}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,editor")]
        public async Task< ActionResult> Edit(string tag, string newTag)
        {
            var tags = _responsitory.GetAll();
            if (!tags.Contains(tag))
            {
                return HttpNotFound();
            }

            if (tags.Contains(newTag))
            {
                return RedirectToAction("index");
            }
            if (!ModelState.IsValid)
            {
                return View(model: newTag);
            }

            if (string.IsNullOrWhiteSpace(newTag))
            {
                ModelState.AddModelError(string.Empty, "New tag value cannot be empty or white space");
                return View(model: tag);
            }
           await _responsitory.EditAsync(tag,newTag);
            return RedirectToAction("index");
        }

        [HttpGet]
        [Route("delete/{tag}")]
        [Authorize(Roles = "admin,editor")]
        public ActionResult Delete(string tag)
        {
            try
            {
                var model = _responsitory.Get(tag);
                return View(model: model);
            }
            catch (KeyNotFoundException)
            {
                return HttpNotFound();
            }
        }


        [HttpPost]
        [Route("delete/{tag}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,editor")]
        public async Task<ActionResult> Delete(string tag,string foo)
        {

            try
            {
                await _responsitory.DeleteAsync(tag);
                return RedirectToAction("index");
            }catch(KeyNotFoundException )
            {
                return HttpNotFound();
            }
         
        }
    }
}