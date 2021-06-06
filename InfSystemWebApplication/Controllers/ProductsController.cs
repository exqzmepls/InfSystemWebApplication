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
    public class ProductsController : Controller
    {
        private IRepository db;

        string reportName = "Products";

        public ProductsController() { db = new Repository(); }

        public ProductsController(IRepository repository) { db = repository; }

        // GET: Products
        [Authorize(Roles = "user")]
        public ActionResult Index()
        {
            var products = db.GetList<Product>();

            return View(products);
        }

        // GET: Sales/Details/5
        [Authorize(Roles = "favored_user")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Find<Product>(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View("Details", product);
        }

        // GET: Products/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (db.GetList<Supplier>().Any())
            {
                ViewBag.CategoryId = new SelectList(db.GetList<ProductCategory>(), "Id", "Name");
                ViewBag.SupplierId = new SelectList(db.GetList<Supplier>(), "Id", "View");
                ViewBag.UnitId = new SelectList(db.GetList<Unit>(), "Id", "Name");
                return View();
            }

            return RedirectToAction("NoSuppliers", new { returnUrl = returnUrl });
        }

        // POST: Products/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Create([Bind(Include = "Id,Name,CategoryId,SupplierId,UnitId")] Product product, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                db.Add(product);
                db.SaveChanges();
                return Redirect(returnUrl ?? "/Home/Index");
                //return RedirectToAction("Index");
            }

            ViewBag.ReturnUrl = returnUrl;

            ViewBag.CategoryId = new SelectList(db.GetList<ProductCategory>(), "Id", "Name", product.CategoryId);
            ViewBag.SupplierId = new SelectList(db.GetList<Supplier>(), "Id", "View", product.SupplierId);
            ViewBag.UnitId = new SelectList(db.GetList<Unit>(), "Id", "Name", product.UnitId);
            return View("Create", product);
        }

        [Authorize(Roles = "admin")]
        public ActionResult NoSuppliers(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost, ActionName("NoSuppliers")]
        [Authorize(Roles = "admin")]
        public ActionResult NoSuppliersCreate(string returnUrl)
        {
            return RedirectToAction("Create", "Suppliers", new { returnUrl = $"/Products/Create?returnUrl={returnUrl}" });
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Find<Product>(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.GetList<ProductCategory>(), "Id", "Name", product.CategoryId);
            ViewBag.SupplierId = new SelectList(db.GetList<Supplier>(), "Id", "View", product.SupplierId);
            ViewBag.UnitId = new SelectList(db.GetList<Unit>(), "Id", "Name", product.UnitId);
            return View(product);
        }

        // POST: Products/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Edit([Bind(Include = "Id,Name,CategoryId,SupplierId,UnitId")] Product product, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                db.Update(product);
                db.SaveChanges();
                return Redirect(returnUrl ?? "/Home/Index");
            }
            ViewBag.CategoryId = new SelectList(db.GetList<ProductCategory>(), "Id", "Name", product.CategoryId);
            ViewBag.SupplierId = new SelectList(db.GetList<Supplier>(), "Id", "View", product.SupplierId);
            ViewBag.UnitId = new SelectList(db.GetList<Unit>(), "Id", "Name", product.UnitId);
            return View("Edit", product);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Find<Product>(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteConfirmed(int id, string returnUrl)
        {
            Product product = db.Find<Product>(id);
            db.Remove(product);
            db.SaveChanges();
            return Redirect(returnUrl ?? "/Home/Index");
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        public ActionResult Download()
        {
            return File(Report.Create(reportName, db.GetList<Product>(), EntityProperty.GetProperties(typeof(Product))), "application/unknown", reportName + "-Report.xlsx");
            //return Report("Products", GetAvailable(), db.Products.ToList());
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

        // Создание grid
        private IGrid<Product> CreateGrid(bool isExport = false)
        {
            var products = db.GetList<Product>(); // список продуктов из БД

            IGrid<Product> grid = new Grid<Product>(products)
            {
                ViewContext = new ViewContext { HttpContext = HttpContext }, // информация о представлении
                Query = Request?.QueryString, // строка с параметрами запроса
                EmptyText = "Нет данных" // текст, если нет данных
            };

            // первый столбец - ID товара
            grid.Columns.Add(model => model.Id).Filterable(GridFilterType.Double).Sortable(true);

            // второй столбец - название товара
            grid.Columns.Add(model => model.Name).Filterable(GridFilterType.Double).Sortable(true);

            // третий столбец - категория товара
            grid.Columns.Add(model => model.Category.View)
                .UsingFilterOptions(GetCategoryOptions(products))
                .Filterable(GridFilterType.Double).Sortable(true);

            // четвертый столбец - поставщик товара
            grid.Columns.Add(model => model.Supplier.View)
                .UsingFilterOptions(GetSupplierOptions(products))
                .Filterable(GridFilterType.Double).Sortable(true);

            // пятый стобец - единица измерения
            grid.Columns.Add(model => model.Unit.View)
                .UsingFilterOptions(GetUnitOptions(products))
                .Filterable(GridFilterType.Double)
                .Sortable(true);

            // если grid не для экспорта, то добавляется столбец с ссылками на действия ("изменить" и "удалить")
            if (!isExport)
                grid.Columns.Add()
                    .RenderedAs(x => new HtmlString($"<a href={GetEditHref(x.Id)}>Изменить</a> | <a href={GetDeleteHref(x.Id)}>Удалить</a>"));

            return grid;
        }

        #region filter options
        private IEnumerable<SelectListItem> GetCategoryOptions(IEnumerable<Product> products)
        {
            Dictionary<int, SelectListItem> result = new Dictionary<int, SelectListItem>
            {
                { 0, new SelectListItem() }
            };

            foreach (var product in products)
            {
                if (!result.ContainsKey(product.CategoryId))
                {
                    result.Add(product.CategoryId, new SelectListItem() { Text = product.Category.View, Value = product.Category.View });
                }
            }

            return result.Values;
        }

        private IEnumerable<SelectListItem> GetUnitOptions(IEnumerable<Product> products)
        {
            Dictionary<int, SelectListItem> result = new Dictionary<int, SelectListItem>
            {
                { 0, new SelectListItem() }
            };

            foreach (var product in products)
            {
                if (!result.ContainsKey(product.UnitId))
                {
                    result.Add(product.UnitId, new SelectListItem() { Text = product.Unit.View, Value = product.Unit.View });
                }
            }

            return result.Values;
        }

        private IEnumerable<SelectListItem> GetSupplierOptions(IEnumerable<Product> products)
        {
            Dictionary<int, SelectListItem> result = new Dictionary<int, SelectListItem>
            {
                { 0, new SelectListItem() }
            };

            foreach (var product in products)
            {
                if (!result.ContainsKey(product.SupplierId))
                {
                    result.Add(product.SupplierId, new SelectListItem() { Text = product.Supplier.View, Value = product.Supplier.View });
                }
            }

            return result.Values;
        }

        #endregion

        private string GetEditHref(int id) => $"\"/Products/Edit/{id}?returnUrl=/Products/Query\"";

        private string GetDeleteHref(int id) => $"\"/Products/Delete/{id}?returnUrl=/Products/Query\"";

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
