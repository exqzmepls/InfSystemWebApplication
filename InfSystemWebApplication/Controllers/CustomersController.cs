using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InfSystemWebApplication.Models;
using InfSystemWebApplication.ReportBuilder;
using InfSystemWebApplication.Repositories;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;

namespace InfSystemWebApplication.Controllers
{
    public class CustomersController : Controller
    {
        private IRepository db;

        string reportName = "Customers";

        public CustomersController() { db = new Repository(); }

        public CustomersController(IRepository repository) { db = repository; }

        // GET: Customers
        [Authorize(Roles = "user")]
        public ActionResult Index()
        {
            return View(db.GetList<Customer>());
        }

        // GET: Sales/Details/5
        [Authorize(Roles = "user")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Find<Customer>(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View("Details", customer);
        }

        // GET: Customers/Create
        [Authorize(Roles = "user")]
        public ActionResult Create(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        // POST: Customers/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "user")]
        public ActionResult Create([Bind(Include = "Id,Name,PhoneNumber")] Customer customer, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                db.Add(customer);
                db.SaveChanges();
                return Redirect(returnUrl ?? "/Home/Index");
            }

            return View("Create", customer);
        }

        // GET: Customers/Edit/5
        [Authorize(Roles = "favored_user")]
        public ActionResult Edit(int? id, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Find<Customer>(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "favored_user")]
        public ActionResult Edit([Bind(Include = "Id,Name,PhoneNumber")] Customer customer, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                db.Update(customer);
                db.SaveChanges();
                return Redirect(returnUrl ?? "/Home/Index");
                //return RedirectToAction("Index");
            }
            return View("Edit", customer);
        }

        // GET: Customers/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Find<Customer>(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteConfirmed(int id, string returnUrl)
        {
            Customer customer = db.Find<Customer>(id);
            db.Remove(customer);
            db.SaveChanges();
            return Redirect(returnUrl ?? "/Home/Index");
            //return RedirectToAction("Index");
        }

        [Authorize(Roles = "user")]
        public ActionResult Download()
        {
            return File(Report.Create(reportName, db.GetList<Customer>(), EntityProperty.GetProperties(typeof(Customer))), "application/unknown", reportName + "-Report.xlsx");
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        public ViewResult Query()
        {
            return View("Query", CreateGrid());
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        public ActionResult Export()
        {
            return File(ReportFromGrid.Create(reportName, CreateGrid(true)), "application/unknown", reportName + "-Report.xlsx");
        }

        private IGrid<Customer> CreateGrid(bool isExport = false)
        {
            IGrid<Customer> grid = new Grid<Customer>(db.GetList<Customer>())
            {
                ViewContext = new ViewContext { HttpContext = HttpContext },
                Query = Request?.QueryString,
                EmptyText = "Нет данных"
            };

            grid.Columns.Add(model => model.Id).Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.Name).Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.PhoneNumber).Filterable(true).Sortable(true);

            if (!isExport)
            {
                grid.Columns.Add().RenderedAs(x => new HtmlString($"<a href={GetEditHref(x.Id)}>Изменить</a> | <a href={GetDeleteHref(x.Id)}>Удалить</a>"));
            }

            return grid;
        }

        private string GetEditHref(int id) => $"\"/Customers/Edit/{id}?returnUrl=/Customers/Query\"";

        private string GetDeleteHref(int id) => $"\"/Customers/Delete/{id}?returnUrl=/Customers/Query\"";

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
