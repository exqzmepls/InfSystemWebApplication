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
    public class ProductsControllerTest
    {
        static Supplier supplier = new Supplier { Id = 311, Name = "name", PhoneNumber = "123" };
        static ProductCategory productCategory = new ProductCategory { Id = 111, Name = "name" };
        static Unit unit = new Unit { Id = 333, Name = "name" };

        List<Product> products = new List<Product> { new Product { Id = 133, SupplierId = 311, Supplier = supplier, CategoryId = 111, Category = productCategory, UnitId = 333, Unit = unit } };
        List<Supplier> suppliers = new List<Supplier> { supplier };
        List<ProductCategory> categories = new List<ProductCategory> { productCategory };

        List<Unit> units = new List<Unit> { unit };
        Mock<IRepository> mock;
        ProductsController controller;

        [TestInitialize]
        public void SetupContext()
        {
            mock = new Mock<IRepository>();
            mock.Setup(x => x.GetList<Product>()).Returns(products);
            mock.Setup(x => x.GetList<Supplier>()).Returns(suppliers);
            mock.Setup(x => x.GetList<ProductCategory>()).Returns(categories);
            mock.Setup(x => x.GetList<Unit>()).Returns(units);
            mock.Setup(x => x.Find<Product>(It.IsAny<object[]>())).Returns<object[]>(k => products.Find(x => x.Id == (int)k[0]));
            mock.Setup(x => x.Find<Supplier>(It.IsAny<object[]>())).Returns<object[]>(k => suppliers.Find(x => x.Id == (int)k[0]));
            controller = new ProductsController(mock.Object);
        }

        [TestMethod]
        public void IndexViewModelNotNull()
        {
            ViewResult result = controller.Index() as ViewResult;

            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOfType(result.Model, typeof(IEnumerable<Product>));
            mock.Verify(x => x.GetList<Product>());
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
            mock.Verify(x => x.Find<Product>(0));
        }

        [TestMethod]
        public void DetailsViewNotNull()
        {
            string expected = "Details";
            ViewResult result = controller.Details(133) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewName);
            mock.Verify(x => x.Find<Product>(133));
        }

        [TestMethod]
        public void Create_NoSuppliers()
        {
            string expected = "NoSuppliers";
            mock.Setup(x => x.GetList<Supplier>()).Returns(new List<Supplier>());
            RedirectToRouteResult result = controller.Create(null) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.RouteValues["action"]);
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
            Product product = new Product();
            controller.ModelState.AddModelError("Id", "error");

            ViewResult result = controller.Create(product, null) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewName);
        }

        [TestMethod]
        public void CreatePostAction_Success()
        {
            string expected = "/Products/Index";
            Product product = new Product();

            RedirectResult result = controller.Create(product, expected) as RedirectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.Url);
            mock.Verify(x => x.Add(product));
            mock.Verify(x => x.SaveChanges());
        }

        [TestMethod]
        public void NoSuppliersViewNotNull()
        {
            string expected = "back2reality";
            ViewResult result = controller.NoSuppliers(expected) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewBag.ReturnUrl as string);
        }

        [TestMethod]
        public void NoSuppliers_Redirect()
        {
            string expectedAction = "Create";
            string expectedController = "Suppliers";
            RedirectToRouteResult result = controller.NoSuppliersCreate(null) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedAction, result.RouteValues["action"]);
            Assert.AreEqual(expectedController, result.RouteValues["controller"]);
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
            mock.Verify(x => x.Find<Product>(0));
        }

        [TestMethod]
        public void EditViewNotNull()
        {
            string expected = "/Products/Index";
            ViewResult result = controller.Edit(133, expected) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewBag.ReturnUrl as string);
            mock.Verify(x => x.Find<Product>(133));
        }

        [TestMethod]
        public void EditPostAction_ModelError()
        {
            string expected = "Edit";
            Product product = new Product();
            controller.ModelState.AddModelError("Id", "error");

            ViewResult result = controller.Edit(product, null) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewName);
        }

        [TestMethod]
        public void EditPostAction_Success()
        {
            string expected = "/Products/Index";
            Product product = new Product();

            RedirectResult result = controller.Edit(product, expected) as RedirectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.Url);
            mock.Verify(x => x.Update(product));
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
            mock.Verify(x => x.Find<Product>(0));
        }

        [TestMethod]
        public void DeleteViewNotNull()
        {
            string expected = "/Products/Index";
            ViewResult result = controller.Delete(133, expected) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewBag.ReturnUrl as string);
            mock.Verify(x => x.Find<Product>(133));
        }

        [TestMethod]
        public void DeletePostAction()
        {
            string expected = "/Products/Index";

            RedirectResult result = controller.DeleteConfirmed(133, expected) as RedirectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.Url);
            mock.Verify(x => x.Find<Product>(133));
            mock.Verify(x => x.SaveChanges());
        }

        [TestMethod]
        public void Download()
        {
            string contentType = "application/unknown";
            string fileName = "Products-Report.xlsx";

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
            string fileName = "Products-Report.xlsx";

            FileContentResult result = controller.Export() as FileContentResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(contentType, result.ContentType);
            Assert.AreEqual(fileName, result.FileDownloadName);
        }
    }
}
