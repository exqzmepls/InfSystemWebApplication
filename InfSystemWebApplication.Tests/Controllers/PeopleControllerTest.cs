using InfSystemWebApplication.Controllers;
using InfSystemWebApplication.Models;
using InfSystemWebApplication.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace InfSystemWebApplication.Tests.Controllers
{
    [TestClass]
    public class PeopleControllerTest
    {
        List<Person> people = new List<Person> { new Person { Id = 13 } };
        Mock<IRepository> mock;
        PeopleController controller;

        [TestInitialize]
        public void SetupContext()
        {
            mock = new Mock<IRepository>();
            mock.Setup(x => x.GetList<Person>()).Returns(people);
            mock.Setup(x => x.Find<Person>(It.IsAny<object[]>())).Returns<object[]>(k => people.Find(x => x.Id == (int)k[0]));
            controller = new PeopleController(mock.Object);
        }

        [TestMethod]
        public void IndexViewModelNotNull()
        {
            ViewResult result = controller.Index() as ViewResult;

            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOfType(result.Model, typeof(IEnumerable<Person>));
            mock.Verify(x => x.GetList<Person>());
        }

        [TestMethod]
        public void CreateViewNotNull()
        {
            string expected = "back2reality";
            ViewResult result = controller.Create(expected) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewBag.ReturnUrl as string);
        }

        [TestMethod]
        public void CreatePostAction_ModelError()
        {
            string expected = "Create";
            Person person = new Person();
            controller.ModelState.AddModelError("Name", "Название модели не установлено");

            ViewResult result = controller.Create(person, null) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewName);
        }

        [TestMethod]
        public void CreatePostAction_Success()
        {
            string expected = "/People/Index";
            Person person = new Person();

            RedirectResult result = controller.Create(person, expected) as RedirectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.Url);
            mock.Verify(x => x.Add(person));
            mock.Verify(x => x.SaveChanges());
        }

        [TestMethod]
        public void Edit_NullId()
        {
            HttpStatusCodeResult result = controller.Edit((int?)null, null) as HttpStatusCodeResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public void Edit_NotFound()
        {
            HttpNotFoundResult result = controller.Edit(0, null) as HttpNotFoundResult;

            Assert.IsNotNull(result);
            mock.Verify(x => x.Find<Person>(0));
        }

        [TestMethod]
        public void EditViewNotNull()
        {
            string expected = "/People/Index";
            ViewResult result = controller.Edit(13, expected) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewBag.ReturnUrl as string);
            mock.Verify(x => x.Find<Person>(13));
        }

        [TestMethod]
        public void EditPostAction_ModelError()
        {
            string expected = "Edit";
            Person person = new Person();
            controller.ModelState.AddModelError("Name", "Название модели не установлено");

            ViewResult result = controller.Edit(person, null) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewName);
        }

        [TestMethod]
        public void EditPostAction_Success()
        {
            string expected = "/People/Index";
            Person person = new Person();

            RedirectResult result = controller.Edit(person, expected) as RedirectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.Url);
            mock.Verify(x => x.Update(person));
            mock.Verify(x => x.SaveChanges());
        }

        [TestMethod]
        public void Delete_NullId()
        {
            int? id = null;
            HttpStatusCodeResult result = controller.Delete(id, null) as HttpStatusCodeResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public void Delete_NotFound()
        {
            HttpNotFoundResult result = controller.Delete(0, null) as HttpNotFoundResult;

            Assert.IsNotNull(result);
            mock.Verify(x => x.Find<Person>(0));
        }

        [TestMethod]
        public void DeleteViewNotNull()
        {
            string expected = "/People/Index";
            ViewResult result = controller.Delete(13, expected) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewBag.ReturnUrl as string);
            mock.Verify(x => x.Find<Person>(13));
        }

        [TestMethod]
        public void DeletePostAction()
        {
            string expected = "/People/Index";

            RedirectResult result = controller.DeleteConfirmed(13, expected) as RedirectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.Url);
            mock.Verify(x => x.Find<Person>(13));
            mock.Verify(x => x.SaveChanges());
        }

        [TestMethod]
        public void Download()
        {
            string contentType = "application/unknown";
            string fileName = "People-Report.xlsx";

            FileContentResult result = controller.Download() as FileContentResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(contentType, result.ContentType);
            Assert.AreEqual(fileName, result.FileDownloadName);
        }

        [TestMethod]
        public void Query()
        {
            string expected = "Query";

            ViewResult result = controller.Query();

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewName);
        }

        [TestMethod]
        public void Export()
        {
            string contentType = "application/unknown";
            string fileName = "People-Report.xlsx";

            FileContentResult result = controller.Export() as FileContentResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(contentType, result.ContentType);
            Assert.AreEqual(fileName, result.FileDownloadName);
        }
    }
}
