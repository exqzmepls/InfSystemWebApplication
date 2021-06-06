using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InfSystemWebApplication.Models;
using InfSystemWebApplication.ReportBuilder;
using InfSystemWebApplication.Repositories;
using NonFactors.Mvc.Grid;

namespace InfSystemWebApplication.Controllers
{
    public class PeopleController : Controller
    {
        private IRepository db;

        string reportName = "People";

        public PeopleController() { db = new Repository(); }

        public PeopleController(IRepository repository) { db = repository; }

        // GET: People
        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            return View(db.GetList<Person>());
        }

        // GET: People/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: People/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Create([Bind(Include = "Id,Surname,Name,MiddleName,DOB,PhoneNumber")] Person person, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                db.Add(person);
                db.SaveChanges();
                return Redirect(returnUrl ?? "/Home/Index");
            }

            return View("Create", person);
        }

        // GET: People/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.Find<Person>(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // POST: People/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Edit([Bind(Include = "Id,Surname,Name,MiddleName,DOB,PhoneNumber")] Person person, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                db.Update(person);
                db.SaveChanges();
                return Redirect(returnUrl ?? "/Home/Index");
            }
            return View("Edit", person);
        }

        // GET: People/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.Find<Person>(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteConfirmed(int id, string returnUrl)
        {
            Person person = db.Find<Person>(id);
            db.Remove(person);
            db.SaveChanges();
            return Redirect(returnUrl ?? "/Home/Index");
        }

        [Authorize(Roles = "admin")]
        public ActionResult Download()
        {
            return File(Report.Create(reportName, db.GetList<Person>(), EntityProperty.GetProperties(typeof(Person))), "application/unknown", reportName + "-Report.xlsx");
        }

        [Authorize(Roles = "admin")]
        public ViewResult Query()
        {
            return View("Query", CreateGrid());
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Export()
        {
            return File(ReportFromGrid.Create(reportName, CreateGrid(true)), "application/unknown", reportName + "-Report.xlsx");
        }

        private IGrid<Person> CreateGrid(bool isExport = false)
        {
            IGrid<Person> grid = new Grid<Person>(db.GetList<Person>())
            {
                ViewContext = new ViewContext { HttpContext = HttpContext },
                Query = Request?.QueryString,
                EmptyText = "Нет данных"
            };

            grid.Columns.Add(model => model.Id).Filterable(GridFilterType.Double).Sortable(true);
            grid.Columns.Add(model => model.Surname).Filterable(GridFilterType.Double).Sortable(true);
            grid.Columns.Add(model => model.Name).Filterable(GridFilterType.Double).Sortable(true);
            grid.Columns.Add(model => model.MiddleName).Filterable(GridFilterType.Double).Sortable(true);
            grid.Columns.Add(model => model.DOB).RenderedAs(model => model.DOB.ToShortDateString()).Filterable(GridFilterType.Double).Sortable(true);
            grid.Columns.Add(model => model.PhoneNumber).Filterable(GridFilterType.Double).Sortable(true);

            if (!isExport)
            {
                grid.Columns.Add().RenderedAs(x => new HtmlString($"<a href={GetEditHref(x.Id)}>Изменить</a> | <a href={GetDeleteHref(x.Id)}>Удалить</a>"));
            }

            return grid;
        }

        private string GetEditHref(int id) => $"\"/People/Edit/{id}?returnUrl=/People/Query\"";

        private string GetDeleteHref(int id) => $"\"/People/Delete/{id}?returnUrl=/People/Query\"";

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
