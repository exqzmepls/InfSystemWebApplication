using InfSystemWebApplication.Controllers;
using InfSystemWebApplication.Models;
using InfSystemWebApplication.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace InfSystemWebApplication.Tests.Controllers
{
    [TestClass]
    public class ProductPricesControllerTest
    {
        static Product product = new Product() { Id = 311, Name = "name" };
        static ProductPrice oldPrice = new ProductPrice() { Id = 111, ProductId = 311, Product = product, SettingDate = DateTime.Now.Date };

        List<ProductPrice> productPrices = new List<ProductPrice> { new ProductPrice() { Id = 133, Product = product, ProductId = 311, SettingDate = DateTime.Now.Date } };
        List<Product> products = new List<Product> { product };
        Mock<IRepository> mock;
        ProductPricesController controller;

        [TestInitialize]
        public void SetupContext()
        {
            product.Prices = new List<ProductPrice> { oldPrice };
            mock = new Mock<IRepository>();
            mock.Setup(x => x.GetList<ProductPrice>()).Returns(productPrices);
            mock.Setup(x => x.GetList<Product>()).Returns(products);
            mock.Setup(x => x.Find<ProductPrice>(It.IsAny<object[]>())).Returns<object[]>(k => productPrices.Find(x => x.Id == (int)k[0]));
            mock.Setup(x => x.Find<Product>(It.IsAny<object[]>())).Returns<object[]>(k => products.Find(x => x.Id == (int)k[0]));
            controller = new ProductPricesController(mock.Object);
        }

        [TestMethod]
        public void IndexViewModelNotNull()
        {
            ViewResult result = controller.Index() as ViewResult;

            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOfType(result.Model, typeof(IEnumerable<ProductPrice>));
            mock.Verify(x => x.GetList<ProductPrice>());
        }

        [TestMethod]
        public void Create_NoProducts()
        {
            string expected = "NoProducts";
            mock.Setup(x => x.GetList<Product>()).Returns(new List<Product>());
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
            ProductPrice productPrice = new ProductPrice() { Id = 133, Product = product, ProductId = 311, SettingDate = DateTime.Now.Date.AddYears(1) };
            controller.ModelState.AddModelError("Id", "error");

            ViewResult result = controller.Create(productPrice, null) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewName);
        }

        [TestMethod]
        public void CreatePostAction_SettingDateExists()
        {
            string expected = "Create";
            ProductPrice productPrice = new ProductPrice() { Id = 133, Product = product, ProductId = 311, SettingDate = DateTime.Now.Date };

            ViewResult result = controller.Create(productPrice, null) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewName);
        }

        [TestMethod]
        public void CreatePostAction_Success()
        {
            string expected = "/ProductPrices/Index";
            ProductPrice productPrice = new ProductPrice() { Id = 133, Product = product, ProductId = 311, SettingDate = DateTime.Now.Date.AddYears(1) };

            RedirectResult result = controller.Create(productPrice, expected) as RedirectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.Url);
            mock.Verify(x => x.Add(productPrice));
            mock.Verify(x => x.SaveChanges());
        }

        [TestMethod]
        public void NoProductsViewNotNull()
        {
            string expected = "back2reality";
            ViewResult result = controller.NoProducts(expected) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewBag.ReturnUrl as string);
        }

        [TestMethod]
        public void NoProducts_Redirect()
        {
            string expectedAction = "Create";
            string expectedController = "Products";
            RedirectToRouteResult result = controller.NoProductsCreate(null) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedAction, result.RouteValues["action"]);
            Assert.AreEqual(expectedController, result.RouteValues["controller"]);
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
            mock.Verify(x => x.Find<ProductPrice>(0));
        }

        [TestMethod]
        public void DeleteViewNotNull()
        {
            string expected = "/ProductPrices/Index";
            ViewResult result = controller.Delete(133, expected) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewBag.ReturnUrl as string);
            mock.Verify(x => x.Find<ProductPrice>(133));
        }

        [TestMethod]
        public void DeletePostAction()
        {
            string expected = "/ProductPrices/Index";

            RedirectResult result = controller.DeleteConfirmed(133, expected) as RedirectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.Url);
            mock.Verify(x => x.Find<ProductPrice>(133));
            mock.Verify(x => x.SaveChanges());
        }

        [TestMethod]
        public void Download()
        {
            string contentType = "application/unknown";
            string fileName = "ProductPrices-Report.xlsx";

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
            string fileName = "ProductPrices-Report.xlsx";

            FileContentResult result = controller.Export() as FileContentResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(contentType, result.ContentType);
            Assert.AreEqual(fileName, result.FileDownloadName);
        }
    }
}
