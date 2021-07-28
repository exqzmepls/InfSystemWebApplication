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
    public class SalesController : Controller
    {
        private IRepository db;

        string reportName = "Sales";

        public SalesController() { db = new Repository(); }

        public SalesController(IRepository repository) { db = repository; }

        // GET: Sales
        [Authorize(Roles = "user")]
        public ActionResult Index()
        {
            var sales = db.GetList<Sale>();
            return View(sales);
        }

        // GET: Sales/Details/5
        [Authorize(Roles = "user")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sale sale = db.Find<Sale>(id);
            if (sale == null)
            {
                return HttpNotFound();
            }
            return View("Details", sale);
        }

        // GET: Sales/Create
        [Authorize(Roles = "user")]
        public ActionResult Create(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            var currentProductPrices = GetCurrentProductPrices(); // текущие цены на каждый товар

            // есть доступные для продажи товары (товар с действующими ценами)
            if (currentProductPrices.Any())
            {
                ViewBag.ProductPriceId = new SelectList(currentProductPrices, "Id", "ProductView"); // список таких товаров

                var employees = GetCurrentWorkingEmployees(); // сотрудники

                // есть сотрудники
                if (employees.Any())
                {
                    var employeesSelectList = GetEmployees();

                    if (employeesSelectList is null) return Content("Невозможно создать продажу.");

                    ViewBag.EmployeeId = employeesSelectList;

                    var customers = db.GetList<Customer>(); // покупатели

                    // есть покупатели
                    if (customers.Any())
                    {
                        ViewBag.CustomerId = new SelectList(customers, "Id", "View"); // список покупателей для представления

                        return View(new Sale() { Date = DateTime.Now }); // продажа (дата = текущий момент)
                    }
                    // нет покупателей
                    return RedirectToAction("NoCustomers", new { returnUrl = returnUrl });
                }
                // нет сотрудников
                return Content("Нет зарегистрированных сотрудников.");
            }
            // нет доступных для продажи товаров
            return Content("Нет доступных товаров для продажи.");
        }

        // POST: Sales/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "user")] 
        public ActionResult Create([Bind(Include = "Id,Date,EmployeeId,CustomerId,SoldProducts")] Sale sale, SoldProduct soldProduct, string action, string returnUrl)
        {
            // ПРИКОЛ С ДОБАВЛЕНИЕМ НОВОГО ТОВАРА ПОСЛЕ УВЕЛИЧЕНИЯ КОЛИЧСЕТВА ПРЕДЫДУЩЕГО
            // СОХРАНЕНИЕ, КОГДА НЕТ ТОВАРОВ?
            if (action == "add")
            {
                if (sale.SoldProducts is null) sale.SoldProducts = new List<SoldProduct>();

                int soldIndex = sale.SoldProducts.FindIndex(x => x.ProductPriceId == soldProduct.ProductPriceId);

                if (soldIndex == -1) sale.SoldProducts.Add(new SoldProduct() { ProductPriceId = soldProduct.ProductPriceId, Amount = soldProduct.Amount });
                else
                {
                    double prevAmount = sale.SoldProducts[soldIndex].Amount;
                    sale.SoldProducts[soldIndex] = new SoldProduct() { ProductPriceId = soldProduct.ProductPriceId, Amount = soldProduct.Amount + prevAmount };
                }
            }
            else
            {
                if (action.Contains("remove"))
                {
                    int id = Convert.ToInt32(action[action.Length - 1].ToString());
                    sale.SoldProducts.RemoveAt(id);
                }
                else
                {
                    if (action == "confirm")
                    {
                        if (ModelState.IsValid)
                        {
                            db.Add(sale);
                            db.SaveChanges();
                            return Redirect(returnUrl ?? "/Home/Index");
                        }
                    }
                }
            }

            ViewBag.CustomerId = new SelectList(db.GetList<Customer>(), "Id", "View", sale.CustomerId);

            ViewBag.ProductPriceId = new SelectList(GetCurrentProductPrices(), "Id", "ProductView");

            var productPrices = db.GetList<ProductPrice>();
            if (sale.SoldProducts != null) foreach (var sp in sale.SoldProducts)
            {
                sp.ProductPrice = productPrices.Single(x => x.Id == sp.ProductPriceId);
            }

            ViewBag.ReturnUrl = returnUrl;

            var employeesSelectList = GetEmployees(sale);

            if (employeesSelectList is null) return Content("Невозможно создать продажу.");

            ViewBag.EmployeeId = employeesSelectList;

            return View(sale);
        }

        //[Authorize(Roles = "admin")]
        //public ActionResult NoEmployees(string returnUrl)
        //{
        //    ViewBag.ReturnUrl = returnUrl;

        //    return View();
        //}

        //[HttpPost, ActionName("NoEmployees")]
        //[Authorize(Roles = "admin")]
        //public ActionResult NoEmployeesCreate(string returnUrl)
        //{
        //    return RedirectToAction("Create", "Employees", new { returnUrl = $"/Sales/Create?returnUrl={returnUrl}" });
        //}

        [Authorize(Roles = "user")]
        public ActionResult NoCustomers(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost, ActionName("NoCustomers")]
        [Authorize(Roles = "user")]
        public ActionResult NoCustomersCreate(string returnUrl)
        {
            return RedirectToAction("Create", "Customers", new { returnUrl = $"/Sales/Create?returnUrl={returnUrl}" });
        }


        // GET: Sales/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id, string returnUrl)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sale sale = db.Find<Sale>(id);
            if (sale == null)
            {
                return HttpNotFound();
            }

            ViewBag.CustomerId = new SelectList(db.GetList<Customer>(), "Id", "View", sale.CustomerId);

            ViewBag.EmployeeId = new SelectList(GetWorkingEmployees(sale.Date), "Id", "View", sale.EmployeeId);

            ViewBag.ReturnUrl = returnUrl;

            return View(sale);
        }

        // POST: Sales/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Edit([Bind(Include = "Id,Date,EmployeeId,CustomerId")] Sale sale, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                db.Update(sale);
                db.SaveChanges();
                return Redirect(returnUrl ?? "/Home/Index");
            }
            ViewBag.CustomerId = new SelectList(db.GetList<Customer>(), "Id", "View", sale.CustomerId);
            ViewBag.EmployeeId = new SelectList(GetWorkingEmployees(sale.Date), "Id", "View", sale.EmployeeId);
            ViewBag.ReturnUrl = returnUrl;
            return View("Edit", sale);
        }

        // GET: Sales/Delete/5
        [Authorize(Roles = "favored_user")]
        public ActionResult Delete(int? id, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sale sale = db.Find<Sale>(id);
            if (sale == null)
            {
                return HttpNotFound();
            }
            return View(sale);
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "favored_user")]
        public ActionResult DeleteConfirmed(int id, string returnUrl)
        {
            Sale sale = db.Find<Sale>(id);
            db.Remove(sale);
            db.SaveChanges();
            return Redirect(returnUrl ?? "/Home/Index");
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        public ActionResult Download()
        {
            return File(Report.Create(reportName, db.GetList<Sale>(), EntityProperty.GetProperties(typeof(Sale))), "application/unknown", reportName + "-Report.xlsx");
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

        private IGrid<Sale> CreateGrid(bool isExport = false)
        {
            var sales = db.GetList<Sale>();

            IGrid<Sale> grid = new Grid<Sale>(sales)
            {
                ViewContext = new ViewContext { HttpContext = HttpContext },
                Query = Request?.QueryString,
                EmptyText = "Нет данных"
            };

            grid.Columns.Add(model => model.Id).Filterable(GridFilterType.Double).Sortable(true);

            grid.Columns.Add(model => model.Customer.View)
                .UsingFilterOptions(GetCustomerOptions(sales))
                .Filterable(GridFilterType.Double)
                .Sortable(true);

            grid.Columns.Add(model => model.Employee.View)
                .UsingFilterOptions(GetEmployeeOptions(sales))
                .Filterable(GridFilterType.Double)
                .Sortable(true);

            grid.Columns.Add(model => model.Date)
                .RenderedAs(model => model.Date.ToShortDateString())
                .Sortable(true).Filterable(GridFilterType.Double);

            grid.Columns.Add(model => model.Total)
                .Sortable(true).Filterable(GridFilterType.Double);


            if (!isExport) grid.Columns.Add().RenderedAs(x => new HtmlString($"<a href={GetEditHref(x.Id)}>Изменить</a> | <a href={GetDeleteHref(x.Id)}>Удалить</a>"));

            return grid;
        }

        #region filter options
        private IEnumerable<SelectListItem> GetEmployeeOptions(IEnumerable<Sale> sales)
        {
            Dictionary<int, SelectListItem> result = new Dictionary<int, SelectListItem>
            {
                { 0, new SelectListItem() }
            };

            foreach (var sale in sales)
            {
                if (!result.ContainsKey(sale.EmployeeId))
                {
                    result.Add(sale.EmployeeId, new SelectListItem() { Text = sale.Employee.View, Value = sale.Employee.View });
                }
            }

            return result.Values;
        }

        private IEnumerable<SelectListItem> GetCustomerOptions(IEnumerable<Sale> sales)
        {
            Dictionary<int, SelectListItem> result = new Dictionary<int, SelectListItem>
            {
                { 0, new SelectListItem() }
            };

            foreach (var sale in sales)
            {
                if (!result.ContainsKey(sale.CustomerId))
                {
                    result.Add(sale.CustomerId, new SelectListItem() { Text = sale.Customer.View, Value = sale.Customer.View });
                }
            }

            return result.Values;
        }

        #endregion

        private string GetEditHref(int id) => $"\"/Sales/Edit/{id}?returnUrl=/Sales/Query\"";

        private string GetDeleteHref(int id) => $"\"/Sales/Delete/{id}?returnUrl=/Sales/Query\"";

        private List<ProductPrice> GetCurrentProductPrices()
        {
            var currentProductPrices = new List<ProductPrice>(); // текущие цены на каждый товар

            foreach (var product in db.GetList<Product>())
            {
                //db.Entry(product).Collection(p => p.Prices).Load();
                var currentPrice = product.CurrentPrice;
                if (currentPrice != null)
                {
                    //db.Entry(currentPrice).Reference(x => x.Product).Load();
                    currentProductPrices.Add(currentPrice);
                }
            }

            return currentProductPrices;
        }

        private SelectList GetEmployees(Sale sale = null)
        {
            if (User is null) return null;

            if (User.IsInRole("admin"))
            {
                if (sale is null) return new SelectList(GetCurrentWorkingEmployees(), "Id", "View");
                return new SelectList(GetCurrentWorkingEmployees(), "Id", "View", sale.EmployeeId);
            }
            else
            {
                Employee currentEmployee = GetCurrentEmployee(); // текущий сотрудник

                if (currentEmployee is null) return null;

                return new SelectList(new List<Employee> { currentEmployee }, "Id", "View"); // может выбрать только сам себя
            }
        }

        private Employee GetCurrentEmployee()
        {
            ApplicationUser user;

            using (ApplicationDbContext applicationDb = new ApplicationDbContext())
            {
                user = applicationDb.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            }

            if (user is null) return null;

            return db.GetList<Employee>().Where(e => e.Id == user.EmployeeId).FirstOrDefault();
        }

        private List<Employee> GetCurrentWorkingEmployees()
        {
            return GetWorkingEmployees(DateTime.Now);
        }

        private List<Employee> GetWorkingEmployees(DateTime date)
        {
            return db.GetList<Employee>().Where(e => e.EntranceDate <= date && (e.LeaveDate == null || e.LeaveDate > date)).ToList();
        }

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
