using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcCMS.Areas.Admin.Controllers;
using Telerik.JustMock;
using MvcCMS.Data;
using System.Web.Mvc;
using MvcCMS.Models;

namespace MvcCMS.Tests.Admin.Controllers
{
    [TestClass]
    public class PostControllerTests
    {
        [TestMethod]
        public void Edit_GetRequestSendsPostToView()
        {
            var id = "test-post";
            var repo = Mock.Create<IPostRepository>();
            var controller = new PostController(repo);

            Mock.Arrange(() => repo.Get(id)).
                Returns(new Post { Id = id });

            var result = (ViewResult)controller.Edit(id);
            var model = (Post)result.Model;

            Assert.AreEqual(id, model.Id);
        }

        [TestMethod]
        public void Edit_GetRequestNotFoundResult()
        {
            var id = "test-post";
            var repo = Mock.Create<IPostRepository>();
            var controller = new PostController(repo);

            Mock.Arrange(() => repo.Get(id)).
                Returns((Post)null);

            var result = controller.Edit(id);
            Assert.IsTrue(result is HttpNotFoundResult);
        }



        [TestMethod]
        public void Edit_PostRequestNotFoundResult()
        {
            var id = "test-post";
            var repo = Mock.Create<IPostRepository>();
            var controller = new PostController(repo);

            Mock.Arrange(() => repo.Get(id)).
                Returns((Post)null);

            var result = controller.Edit(id, new Post()).Result;
            Assert.IsTrue(result is HttpNotFoundResult);
        }
        [TestMethod]
        public void Edit_PostRequestSendsPostToView()
        {
            var id = "test-post";
            var repo = Mock.Create<IPostRepository>();
            var controller = new PostController(repo);

            Mock.Arrange(() => repo.Get(id)).
                Returns(new Post { Id = id });

            controller.ViewData.ModelState.AddModelError("key", "Model state is invalid");

            var result = (ViewResult)controller.Edit(id, new Post()
            {
                Id = "test-post-2"
            }).Result;
            var model = (Post)result.Model;

            Assert.AreEqual("test-post-2", model.Id);
        }

        [TestMethod]
        public  void Edit_PostRequestCallsEditAndRedirects()
        {
           
            var repo = Mock.Create<IPostRepository>();
            var controller = new PostController(repo);

            Mock.Arrange( () => repo.EditAsync(Arg.IsAny<string>(), Arg.IsAny<Post>())).
                MustBeCalled();

            var result = controller.Edit("foo", new Post() {  Id = "test-post-2" }).Result;
            Mock.Assert(repo);
            Assert.IsTrue(result is RedirectToRouteResult);
        }
    }
}
