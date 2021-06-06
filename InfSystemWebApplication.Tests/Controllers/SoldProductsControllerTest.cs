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
    public class SoldProductsControllerTest
    {
        List<SoldProduct> soldProducts = new List<SoldProduct> { new SoldProduct() };
        Mock<IRepository> mock;
        SoldProductsController controller;

        [TestInitialize]
        public void SetupContext()
        {
            mock = new Mock<IRepository>();
            mock.Setup(x => x.GetList<SoldProduct>()).Returns(soldProducts);
            controller = new SoldProductsController(mock.Object);
        }

        [TestMethod]
        public void IndexViewModelNotNull()
        {
            ViewResult result = controller.Index() as ViewResult;

            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOfType(result.Model, typeof(IEnumerable<SoldProduct>));
            mock.Verify(x => x.GetList<SoldProduct>());
        }

        [TestMethod]
        public void Download()
        {
            string contentType = "application/unknown";
            string fileName = "SoldProducts-Report.xlsx";

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
            string fileName = "SoldProducts-Report.xlsx";

            FileContentResult result = controller.Export() as FileContentResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(contentType, result.ContentType);
            Assert.AreEqual(fileName, result.FileDownloadName);
        }
    }
}
