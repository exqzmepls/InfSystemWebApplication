using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using InfSystemWebApplication.Models;
using InfSystemWebApplication.ReportBuilder;
using InfSystemWebApplication.Repositories;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using NonFactors.Mvc.Grid;

namespace InfSystemWebApplication.Controllers
{
    public class EmployeesController : Controller
    {
        private IRepository db;

        string reportName = "Employees";

        public EmployeesController() { db = new Repository(); }

        public EmployeesController(IRepository repository) { db = repository; }

        // GET: Employees
        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            var employees = db.GetList<Employee>(); /*db.Employees.Include(e => e.Person).Include(e => e.Position);*/
            return View(employees);
        }

        // GET: Sales/Details/5
        [Authorize(Roles = "admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Find<Employee>(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View("Details", employee);
        }

        // GET: Employees/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (db.GetList<Person>().Any())
            {
                ViewBag.PersonId = new SelectList(db.GetList<Person>(), "Id", "View");
                ViewBag.PositionId = new SelectList(db.GetList<Position>(), "Id", "Name");
                return View();
            }

            return RedirectToAction("NoPeople", new { returnUrl = returnUrl });
        }

        // POST: Employees/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Create([Bind(Include = "Id,EntranceDate,LeaveDate,PersonId,PositionId")] Employee employee, string returnUrl)
        {
            if (employee.LeaveDate != null && employee.EntranceDate > employee.LeaveDate)
            {
                ModelState.AddModelError("LeaveDate", "Дата ухода с работы должна быть позже даты начала работы");
            }

            if (ModelState.IsValid)
            {
                db.Add(employee);
                db.SaveChanges();
                return Redirect(returnUrl ?? "/Home/Index");
            }

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.PersonId = new SelectList(db.GetList<Person>(), "Id", "View", employee.PersonId);
            ViewBag.PositionId = new SelectList(db.GetList<Position>(), "Id", "Name", employee.PositionId);
            return View("Create", employee);
        }

        [Authorize(Roles = "admin")]
        public ActionResult NoPeople(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost, ActionName("NoPeople")]
        [Authorize(Roles = "admin")]
        public ActionResult NoPeopleCreate(string returnUrl)
        {
            return RedirectToAction("Create", "People", new { returnUrl = $"/Employees/Create?returnUrl={returnUrl}" });
        }


        // GET: Employees/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id, string returnUrl)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Find<Employee>(id);
            if (employee == null)
            {
                return HttpNotFound();
            }

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.PersonId = new SelectList(db.GetList<Person>(), "Id", "View", employee.PersonId);
            ViewBag.PositionId = new SelectList(db.GetList<Position>(), "Id", "Name", employee.PositionId);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Edit([Bind(Include = "Id,EntranceDate,LeaveDate,PersonId,PositionId")] Employee employee, string returnUrl)
        {
            if (employee.LeaveDate != null && employee.EntranceDate > employee.LeaveDate)
            {
                ModelState.AddModelError("LeaveDate", "Дата ухода с работы должна быть позже даты начала работы");
            }

            if (ModelState.IsValid)
            {
                db.Update(employee);
                db.SaveChanges();
                return Redirect(returnUrl ?? "/Home/Index");
            }

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.PersonId = new SelectList(db.GetList<Person>(), "Id", "View", employee.PersonId);
            ViewBag.PositionId = new SelectList(db.GetList<Position>(), "Id", "Name", employee.PositionId);
            return View("Edit", employee);
        }

        // GET: Employees/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id, string returnUrl)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Find<Employee>(id);
            if (employee == null)
            {
                return HttpNotFound();
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteConfirmed(int id, string returnUrl)
        {
            Employee employee = db.Find<Employee>(id);
            db.Remove(employee);
            db.SaveChanges();

            using (ApplicationDbContext applicationDb = new ApplicationDbContext())
            {
                var user = applicationDb.Users.FirstOrDefault(u => u.EmployeeId == id);
                if (user != null)
                {
                    applicationDb.Users.Remove(user);
                    applicationDb.SaveChanges();
                }
            }

            return Redirect(returnUrl ?? "/Home/Index");
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Download()
        {
            return File(Report.Create(reportName, db.GetList<Employee>(), EntityProperty.GetProperties(typeof(Employee))), "application/unknown", reportName + "-Report.xlsx");
        }

        [HttpGet]
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

        private IGrid<Employee> CreateGrid(bool isExport = false)
        {
            var employees = db.GetList<Employee>();

            IGrid<Employee> grid = new Grid<Employee>(employees)
            {
                ViewContext = new ViewContext { HttpContext = HttpContext },
                Query = Request?.QueryString,
                EmptyText = "Нет данных"
            };

            grid.Columns.Add(model => model.Id).Filterable(true).Sortable(true);

            grid.Columns.Add(model => model.Position.View)
                .UsingFilterOptions(GetPositionOptions(employees))
                .Filterable(GridFilterType.Multi)
                .Sortable(true);

            grid.Columns.Add(model => model.Person.View)
                .UsingFilterOptions(GetPersonOptions(employees))
                .Filterable(GridFilterType.Multi)
                .Sortable(true);

            grid.Columns.Add(model => model.EntranceDate)
                .RenderedAs(model => model.EntranceDate.ToShortDateString())
                .Sortable(true).Filterable(true);

            grid.Columns.Add(model => model.LeaveDate)
                .RenderedAs(model => model.LeaveDate?.ToShortDateString())
                .Sortable(true).Filterable(true);

            if (!isExport) grid.Columns.Add().RenderedAs(x => new HtmlString($"<a href={GetEditHref(x.Id)}>Изменить</a> | <a href={GetDeleteHref(x.Id)}>Удалить</a>"));

            return grid;
        }

        #region filter options
        private IEnumerable<SelectListItem> GetPositionOptions(IEnumerable<Employee> employees)
        {
            Dictionary<int, SelectListItem> result = new Dictionary<int, SelectListItem>
            {
                { 0, new SelectListItem() }
            };

            foreach (var employee in employees)
            {
                if (!result.ContainsKey(employee.PositionId))
                {
                    result.Add(employee.PositionId, new SelectListItem() { Text = employee.Position.View, Value = employee.Position.View });
                }
            }

            return result.Values;
        }

        private IEnumerable<SelectListItem> GetPersonOptions(IEnumerable<Employee> employees)
        {
            Dictionary<int, SelectListItem> result = new Dictionary<int, SelectListItem>
            {
                { 0, new SelectListItem() }
            };

            foreach (var employee in employees)
            {
                if (!result.ContainsKey(employee.PersonId))
                {
                    result.Add(employee.PersonId, new SelectListItem() { Text = employee.Person.View, Value = employee.Person.View });
                }
            }

            return result.Values;
        }

        #endregion

        private string GetEditHref(int id) => $"\"/Employees/Edit/{id}?returnUrl=/Employees/Query\"";

        private string GetDeleteHref(int id) => $"\"/Employees/Delete/{id}?returnUrl=/Employees/Query\"";


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
