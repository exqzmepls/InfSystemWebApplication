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
    public class CustomersControllerTest
    {
        List<Customer> customers = new List<Customer> { new Customer { Id = 13 } };
        Mock<IRepository> mock;
        CustomersController controller;

        [TestInitialize]
        public void SetupContext()
        {
            mock = new Mock<IRepository>();
            mock.Setup(x => x.GetList<Customer>()).Returns(customers);
            mock.Setup(x => x.Find<Customer>(It.IsAny<object[]>())).Returns<object[]>(k => customers.Find(x => x.Id == (int)k[0]));
            controller = new CustomersController(mock.Object);
        }

        [TestMethod]
        public void IndexViewModelNotNull()
        {
            ViewResult result = controller.Index() as ViewResult;

            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOfType(result.Model, typeof(IEnumerable<Customer>));
            mock.Verify(x => x.GetList<Customer>());
        }

        [TestMethod]
        public void Details_NullId()
        {
            HttpStatusCodeResult result = controller.Details(null) as HttpStatusCodeResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public void Details_NotFound()
        {
            HttpNotFoundResult result = controller.Details(0) as HttpNotFoundResult;

            Assert.IsNotNull(result);
            mock.Verify(x => x.Find<Customer>(0));
        }

        [TestMethod]
        public void DetailsViewNotNull()
        {
            string expected = "Details";
            ViewResult result = controller.Details(13) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewName);
            mock.Verify(x => x.Find<Customer>(13));
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
            Customer customer = new Customer();
            controller.ModelState.AddModelError("Name", "Название модели не установлено");

            ViewResult result = controller.Create(customer, null) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewName);
        }

        [TestMethod]
        public void CreatePostAction_Success()
        {
            string expected = "/Customers/Index";
            Customer customer = new Customer();

            ActionResult r = controller.Create(customer, expected);

            RedirectResult result = r as RedirectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.Url);
            mock.Verify(x => x.Add(customer));
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
            mock.Verify(x => x.Find<Customer>(0));
        }

        [TestMethod]
        public void EditViewNotNull()
        {
            string expected = "/Customers/Index";
            ViewResult result = controller.Edit(13, expected) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewBag.ReturnUrl as string);
            mock.Verify(x => x.Find<Customer>(13));
        }

        [TestMethod]
        public void EditPostAction_ModelError()
        {
            string expected = "Edit";
            Customer customer = new Customer();
            controller.ModelState.AddModelError("Name", "Название модели не установлено");

            ViewResult result = controller.Edit(customer, null) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewName);
        }

        [TestMethod]
        public void EditPostAction_Success()
        {
            string expected = "/Suppliers/Index";
            Customer customer = new Customer();

            RedirectResult result = controller.Edit(customer, expected) as RedirectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.Url);
            mock.Verify(x => x.Update(customer));
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
            mock.Verify(x => x.Find<Customer>(0));
        }

        [TestMethod]
        public void DeleteViewNotNull()
        {
            string expected = "/Customers/Index";
            ViewResult result = controller.Delete(13, expected) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewBag.ReturnUrl as string);
            mock.Verify(x => x.Find<Customer>(13));
        }

        [TestMethod]
        public void DeletePostAction()
        {
            string expected = "/Customers/Index";

            RedirectResult result = controller.DeleteConfirmed(13, expected) as RedirectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.Url);
            mock.Verify(x => x.Find<Customer>(13));
            mock.Verify(x => x.SaveChanges());
        }

        [TestMethod]
        public void Download()
        {
            string contentType = "application/unknown";
            string fileName = "Customers-Report.xlsx";

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
            string fileName = "Customers-Report.xlsx";

            FileContentResult result = controller.Export() as FileContentResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(contentType, result.ContentType);
            Assert.AreEqual(fileName, result.FileDownloadName);
        }
    }
}
