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
    public class EmployeesControllerTest
    {
        List<Employee> employees = new List<Employee> { new Employee { Id = 133 } };
        List<Person> people = new List<Person> { new Person { Id = 311 } };
        List<Position> positions = new List<Position> { new Position { Id = 111 } };
        Mock<IRepository> mock;
        EmployeesController controller;

        [TestInitialize]
        public void SetupContext()
        {
            mock = new Mock<IRepository>();
            mock.Setup(x => x.GetList<Employee>()).Returns(employees);
            mock.Setup(x => x.GetList<Person>()).Returns(people);
            mock.Setup(x => x.GetList<Position>()).Returns(positions);
            mock.Setup(x => x.Find<Employee>(It.IsAny<object[]>())).Returns<object[]>(k => employees.Find(x => x.Id == (int)k[0]));
            mock.Setup(x => x.Find<Person>(It.IsAny<object[]>())).Returns<object[]>(k => people.Find(x => x.Id == (int)k[0]));
            controller = new EmployeesController(mock.Object);
        }

        [TestMethod]
        public void IndexViewModelNotNull()
        {
            ViewResult result = controller.Index() as ViewResult;

            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOfType(result.Model, typeof(IEnumerable<Employee>));
            mock.Verify(x => x.GetList<Employee>());
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
            mock.Verify(x => x.Find<Employee>(0));
        }

        [TestMethod]
        public void DetailsViewNotNull()
        {
            string expected = "Details";
            ViewResult result = controller.Details(133) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewName);
            mock.Verify(x => x.Find<Employee>(133));
        }

        [TestMethod]
        public void Create_NoPeople()
        {
            string expected = "NoPeople";
            mock.Setup(x => x.GetList<Person>()).Returns(new List<Person>());
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
            Employee employee = new Employee();
            controller.ModelState.AddModelError("Id", "error");

            ViewResult result = controller.Create(employee, null) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewName);
        }

        [TestMethod]
        public void CreatePostAction_Success()
        {
            string expected = "/Employees/Index";
            Employee employee = new Employee();

            RedirectResult result = controller.Create(employee, expected) as RedirectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.Url);
            mock.Verify(x => x.Add(employee));
            mock.Verify(x => x.SaveChanges());
        }

        [TestMethod]
        public void NoPeopleViewNotNull()
        {
            string expected = "back2reality";
            ViewResult result = controller.NoPeople(expected) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewBag.ReturnUrl as string);
        }

        [TestMethod]
        public void NoPeople_Redirect()
        {
            string expectedAction = "Create";
            string expectedController = "People";
            RedirectToRouteResult result = controller.NoPeopleCreate(null) as RedirectToRouteResult;

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
            mock.Verify(x => x.Find<Employee>(0));
        }

        [TestMethod]
        public void EditViewNotNull()
        {
            string expected = "/Employees/Index";
            ViewResult result = controller.Edit(133, expected) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewBag.ReturnUrl as string);
            mock.Verify(x => x.Find<Employee>(133));
        }

        [TestMethod]
        public void EditPostAction_ModelError()
        {
            string expected = "Edit";
            Employee employee = new Employee();
            controller.ModelState.AddModelError("Id", "error");

            ViewResult result = controller.Edit(employee, null) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewName);
        }

        [TestMethod]
        public void EditPostAction_Success()
        {
            string expected = "/Employees/Index";
            Employee employee = new Employee();

            RedirectResult result = controller.Edit(employee, expected) as RedirectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.Url);
            mock.Verify(x => x.Update(employee));
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
            mock.Verify(x => x.Find<Employee>(0));
        }

        [TestMethod]
        public void DeleteViewNotNull()
        {
            string expected = "/Employees/Index";
            ViewResult result = controller.Delete(133, expected) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.ViewBag.ReturnUrl as string);
            mock.Verify(x => x.Find<Employee>(133));
        }

        [TestMethod]
        public void DeletePostAction()
        {
            string expected = "/Employess/Index";

            RedirectResult result = controller.DeleteConfirmed(133, expected) as RedirectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.Url);
            mock.Verify(x => x.Find<Employee>(133));
            mock.Verify(x => x.SaveChanges());
        }

        [TestMethod]
        public void Download()
        {
            string contentType = "application/unknown";
            string fileName = "Employees-Report.xlsx";

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
            string fileName = "Employees-Report.xlsx";

            FileContentResult result = controller.Export() as FileContentResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(contentType, result.ContentType);
            Assert.AreEqual(fileName, result.FileDownloadName);
        }
    }
}
