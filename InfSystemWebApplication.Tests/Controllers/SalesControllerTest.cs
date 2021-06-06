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
    public class SalesControllerTest
    {
        //static Customer customer = new Customer { Id = 311, Name = "name", PhoneNumber = "123" };
        //static Employee employee = new Employee { Id = 111 };
        //static Product product = new Product() { Id = 311, Name = "name" };
        //static ProductPrice price = new ProductPrice() { Id = 111, ProductId = 311, Product = product, SettingDate = DateTime.Now.Date.AddDays(-1) };

        Mock<IRepository> mock;
        SalesController controller;

        [TestInitialize]
        public void SetupContext()
        {
            Position admin = new Position { Id = (int)PositionId.ADMIN, Name = "admin" };
            Position assistant = new Position { Id = (int)PositionId.ASSISTANT, Name = "assistant" };

            Customer customer = new Customer { Id = 1, Name = "name", PhoneNumber = "123" };
            Supplier supplier = new Supplier { Id = 1, Name = "name", PhoneNumber = "123" };

            Person person1 = new Person { Id = 1, Name = "name", Surname = "sur", MiddleName = "mn", DOB = DateTime.Now.AddYears(-28), PhoneNumber = "321" };
            Person person2 = new Person { Id = 2, Name = "n", Surname = "s", MiddleName = "m", DOB = DateTime.Now.AddYears(-22), PhoneNumber = "321123" };

            Unit unit = new Unit { Id = 1, Name = "unit", ShortName = "u" };

            Employee employee1 = new Employee { Id = 1, Person = person1, PersonId = person1.Id, Position = admin, PositionId = admin.Id, EntranceDate = DateTime.Now.Date.AddYears(-1), LeaveDate = null };
            Employee employee2 = new Employee { Id = 2, Person = person2, PersonId = person2.Id, Position = assistant, PositionId = assistant.Id, EntranceDate = DateTime.Now.Date.AddYears(-2), LeaveDate = DateTime.Now.Date.AddMonths(1) };
            person1.Employees = new List<Employee> { employee1 };
            person2.Employees = new List<Employee> { employee2 };
            admin.Employees = new List<Employee> { employee1 };
            assistant.Employees = new List<Employee> { employee2 };

            Product product1 = new Product() { Id = 1, Name = "name1", Supplier = supplier, SupplierId = supplier.Id, Unit = unit, UnitId = unit.Id };
            Product product2 = new Product() { Id = 2, Name = "name2", Supplier = supplier, SupplierId = supplier.Id, Unit = unit, UnitId = unit.Id };
            supplier.Products = new List<Product> { product1, product2 };
            unit.Products = new List<Product> { product1, product2 };

            ProductPrice price1 = new ProductPrice() { Id = 1, ProductId = product1.Id, Product = product1, SettingDate = DateTime.Now.Date.AddDays(-1) };
            ProductPrice price2 = new ProductPrice() { Id = 2, ProductId = product2.Id, Product = product2, SettingDate = DateTime.Now.Date.AddDays(-1) };
            product1.Prices = new List<ProductPrice> { price1 };
            product2.Prices = new List<ProductPrice> { price2 };

            List<Sale> sales = new List<Sale> { new Sale { Id = 133, CustomerId = customer.Id, Customer = customer } };
            List<Customer> customers = new List<Customer> { customer };
            List<Employee> employees = new List<Employee> { employee1, employee2 };
            List<ProductPrice> productPrices = new List<ProductPrice> { price1, price2 };
            List<Product> products = new List<Product> { product1, product2 };


            mock = new Mock<IRepository>();
            mock.Setup(x => x.GetList<Sale>()).Returns(sales);
            mock.Setup(x => x.GetList<Customer>()).Returns(customers);
            mock.Setup(x => x.GetList<Employee>()).Returns(employees);
            mock.Setup(x => x.GetList<ProductPrice>()).Returns(productPrices);
            mock.Setup(x => x.GetList<Product>()).Returns(products);
            mock.Setup(x => x.Find<Sale>(It.IsAny<object[]>())).Returns<object[]>(k => sales.Find(x => x.Id == (int)k[0]));
            mock.Setup(x => x.Find<Customer>(It.IsAny<object[]>())).Returns<object[]>(k => customers.Find(x => x.Id == (int)k[0]));
            controller = new SalesController(mock.Object);
        }

        [TestMethod]
        public void IndexViewModelNotNull()
        {
            ViewResult result = controller.Index() as ViewResult;

            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOfType(result.Model, typeof(IEnumerable<Sale>));
            mock.Verify(x => x.GetList<Sale>());
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
            mock.Verify(x => x.Find<Sale>(0));
        }

        [TestMethod]
        public void DetailsViewNotNull()
        {
            string expected = "Details";
            ViewResult result = controller.Details(133) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewName);
            mock.Verify(x => x.Find<Sale>(133));
        }

        [TestMethod]
        public void Create_NoCustomers()
        {
            //string expected = "NoCustomers";
            mock.Setup(x => x.GetList<Customer>()).Returns(new List<Customer>());

            ContentResult result = controller.Create(null) as ContentResult;

            Assert.IsNotNull(result);

            //RedirectToRouteResult result = controller.Create(null) as RedirectToRouteResult;

            //Assert.IsNotNull(result);
            //Assert.AreEqual(expected, result.RouteValues["action"]);
        }

        [TestMethod]
        public void CreateViewNotNull()
        {
            string expected = "back2reality";
            //ViewResult result = controller.Create(expected) as ViewResult;

            ContentResult result = controller.Create(expected) as ContentResult;

            Assert.IsNotNull(result);

            //Assert.IsNotNull(result);
            //Assert.AreEqual(expected, result.ViewBag.ReturnUrl as string);
        }

        [TestMethod]
        public void CreatePostAction_ModelError()
        {
            //string expected = "Create";
            Sale sale = new Sale();
            SoldProduct soldProduct = new SoldProduct();
            string action = "confirm";
            controller.ModelState.AddModelError("Id", "error");

            ContentResult result = controller.Create(sale, soldProduct, action, null) as ContentResult;

            Assert.IsNotNull(result);

            //ViewResult result = controller.Create(sale, soldProduct, action, null) as ViewResult;

            //Assert.IsNotNull(result);
            //Assert.AreEqual(expected, result.ViewName);
        }

        [TestMethod]
        public void CreatePostAction_Confirm()
        {
            string expected = "/Sales/Index";
            Sale sale = new Sale();
            SoldProduct soldProduct = new SoldProduct();
            string action = "confirm";

            RedirectResult result = controller.Create(sale, soldProduct, action, expected) as RedirectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.Url);
            mock.Verify(x => x.Add(sale));
            mock.Verify(x => x.SaveChanges());
        }

        [TestMethod]
        public void CreatePostAction_AddNew()
        {
            //string expected = "Create";
            Sale sale = new Sale();
            SoldProduct soldProduct = new SoldProduct() { Amount = 10, ProductPriceId = 1 };
            string action = "add";

            ContentResult result = controller.Create(sale, soldProduct, action, null) as ContentResult;

            Assert.IsNotNull(result);

            //ViewResult result = controller.Create(sale, soldProduct, action, null) as ViewResult;

            //Assert.IsNotNull(result);
            //Assert.AreEqual(expected, result.ViewName);
        }

        [TestMethod]
        public void CreatePostAction_Add()
        {
            //string expected = "Create";
            Sale sale = new Sale() { SoldProducts = new List<SoldProduct> { new SoldProduct() { Amount = 10, ProductPriceId = 1 } } };
            SoldProduct soldProduct = new SoldProduct() { Amount = 10, ProductPriceId = 1 };
            string action = "add";

            ContentResult result = controller.Create(sale, soldProduct, action, null) as ContentResult;

            Assert.IsNotNull(result);

            //ViewResult result = controller.Create(sale, soldProduct, action, null) as ViewResult;

            //Assert.IsNotNull(result);
            //Assert.AreEqual(expected, result.ViewName);
        }

        [TestMethod]
        public void CreatePostAction_Remove()
        {
            //string expected = "Create";
            Sale sale = new Sale() { SoldProducts = new List<SoldProduct> { new SoldProduct() { Amount = 10, ProductPriceId = 1 } } };
            SoldProduct soldProduct = new SoldProduct() { Amount = 10, ProductPriceId = 1 };
            string action = "remove0";

            ContentResult result = controller.Create(sale, soldProduct, action, null) as ContentResult;

            Assert.IsNotNull(result);

            //ViewResult result = controller.Create(sale, soldProduct, action, null) as ViewResult;

            //Assert.IsNotNull(result);
            //Assert.AreEqual(expected, result.ViewName);
        }

        [TestMethod]
        public void NoCustomersViewNotNull()
        {
            string expected = "back2reality";
            ViewResult result = controller.NoCustomers(expected) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewBag.ReturnUrl as string);
        }

        [TestMethod]
        public void NoCustomers_Redirect()
        {
            string expectedAction = "Create";
            string expectedController = "Customers";
            RedirectToRouteResult result = controller.NoCustomersCreate(null) as RedirectToRouteResult;

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
            mock.Verify(x => x.Find<Sale>(0));
        }

        [TestMethod]
        public void EditViewNotNull()
        {
            string expected = "/Sales/Index";
            ViewResult result = controller.Edit(133, expected) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewBag.ReturnUrl as string);
            mock.Verify(x => x.Find<Sale>(133));
        }

        [TestMethod]
        public void EditPostAction_ModelError()
        {
            string expected = "Edit";
            Sale sale = new Sale();
            controller.ModelState.AddModelError("Id", "error");

            ViewResult result = controller.Edit(sale, null) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewName);
        }

        [TestMethod]
        public void EditPostAction_Success()
        {
            string expected = "/Sales/Index";
            Sale sale = new Sale();

            RedirectResult result = controller.Edit(sale, expected) as RedirectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.Url);
            mock.Verify(x => x.Update(sale));
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
            mock.Verify(x => x.Find<Sale>(0));
        }

        [TestMethod]
        public void DeleteViewNotNull()
        {
            string expected = "/Sales/Index";
            ViewResult result = controller.Delete(133, expected) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewBag.ReturnUrl as string);
            mock.Verify(x => x.Find<Sale>(133));
        }

        [TestMethod]
        public void DeletePostAction()
        {
            string expected = "/Sales/Index";

            RedirectResult result = controller.DeleteConfirmed(133, expected) as RedirectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.Url);
            mock.Verify(x => x.Find<Sale>(133));
            mock.Verify(x => x.SaveChanges());
        }

        [TestMethod]
        public void Download()
        {
            string contentType = "application/unknown";
            string fileName = "Sales-Report.xlsx";

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
            string fileName = "Sales-Report.xlsx";

            FileContentResult result = controller.Export() as FileContentResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(contentType, result.ContentType);
            Assert.AreEqual(fileName, result.FileDownloadName);
        }
    }
}
