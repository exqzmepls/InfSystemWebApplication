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
    public class ProductPricesController : Controller
    {
        private IRepository db;

        string reportName = "ProductPrices";

        public ProductPricesController() { db = new Repository(); }

        public ProductPricesController(IRepository repository) { db = repository; }

        // GET: ProductPrices
        [Authorize(Roles = "favored_user")]
        public ActionResult Index()
        {
            var productPrices = db.GetList<ProductPrice>();
            return View(productPrices);
        }

        //// GET: ProductPrices/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ProductPrice productPrice = db.ProductPrices.Find(id);
        //    if (productPrice == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(productPrice);
        //}

        // GET: ProductPrices/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (db.GetList<Product>().Any())
            {
                ViewBag.ProductId = new SelectList(db.GetList<Product>(), "Id", "View");
                return View();
            }

            return RedirectToAction("NoProducts", new { returnUrl = "/ProductPrices/Create" });
        }

        // POST: ProductPrices/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Create([Bind(Include = "Id,ValuePerOneUnit,SettingDate,ProductId")] ProductPrice productPrice, string returnUrl)
        {
            var product = db.Find<Product>(productPrice.ProductId);
            //db.Entry(product).Collection(p => p.Prices).Load();

            //if (product.Prices.Where(x => x.SettingDate.Date == productPrice.SettingDate.Date).Any())
            //{
            //    ModelState.AddModelError("SettingDate", "На данную дату уже назначена цена");
            //}

            if (product.ContainsPriceOnDate(productPrice.SettingDate))
            {
                ModelState.AddModelError("SettingDate", "На данную дату уже назначена цена");
            }

            if (ModelState.IsValid)
            {
                db.Add(productPrice);
                db.SaveChanges();
                return Redirect(returnUrl ?? "/Home/Index");
            }

            ViewBag.ProductId = new SelectList(db.GetList<Product>(), "Id", "View", productPrice.ProductId);
            return View("Create", productPrice);
        }

        [Authorize(Roles = "admin")]
        public ActionResult NoProducts(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost, ActionName("NoProducts")]
        [Authorize(Roles = "admin")]
        public ActionResult NoProductsCreate(string returnUrl)
        {
            return RedirectToAction("Create", "Products", new { returnUrl = $"/ProductPrices/Create?returnUrl={returnUrl}" });
        }

        //// GET: ProductPrices/Edit/5
        //[Authorize(Roles = "admin")]
        //public ActionResult Edit(int? id, string returnUrl)
        //{
        //    ViewBag.ReturnUrl = returnUrl;

        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ProductPrice productPrice = db.ProductPrices.Find(id);
        //    if (productPrice == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.ProductId = new SelectList(db.Products, "Id", "View", productPrice.ProductId);
        //    return View(productPrice);
        //}

        //// POST: ProductPrices/Edit/5
        //// Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        //// сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "admin")]
        //public ActionResult Edit([Bind(Include = "Id,ValuePerOneUnit,SettingDate,ProductId")] ProductPrice productPrice, string returnUrl)
        //{
        //    var product = db.Products.Find(productPrice.ProductId);
        //    db.Entry(product).Collection(p => p.Prices).Load();

        //    if (product.ContainsPriceOnDate(productPrice.SettingDate))
        //    {
        //        ModelState.AddModelError("SettingDate", "На данную дату уже назначена цена");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(productPrice).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return Redirect(returnUrl ?? "/Home/Index");
        //    }
        //    ViewBag.ProductId = new SelectList(db.Products, "Id", "View", productPrice.ProductId);
        //    return View(productPrice);
        //}

        // GET: ProductPrices/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductPrice productPrice = db.Find<ProductPrice>(id);
            if (productPrice == null)
            {
                return HttpNotFound();
            }
            return View(productPrice);
        }

        // POST: ProductPrices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteConfirmed(int id, string returnUrl)
        {
            ProductPrice productPrice = db.Find<ProductPrice>(id);
            db.Remove(productPrice);
            db.SaveChanges();
            return Redirect(returnUrl ?? "/Home/Index");
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Download()
        {
            return File(Report.Create(reportName, db.GetList<ProductPrice>(), EntityProperty.GetProperties(typeof(ProductPrice))), "application/unknown", reportName + "-Report.xlsx");
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

        private IGrid<ProductPrice> CreateGrid(bool isExport = false)
        {
            var productPrices = db.GetList<ProductPrice>();

            IGrid<ProductPrice> grid = new Grid<ProductPrice>(productPrices)
            {
                ViewContext = new ViewContext { HttpContext = HttpContext },
                Query = Request?.QueryString,
                EmptyText = "Нет данных"
            };

            grid.Columns.Add(model => model.Id).Filterable(GridFilterType.Double).Sortable(true);

            grid.Columns.Add(model => model.Product.View)
                .UsingFilterOptions(GetProductOptions(productPrices))
                .Filterable(GridFilterType.Double)
                .Sortable(true);

            grid.Columns.Add(model => model.ValuePerOneUnit).Filterable(GridFilterType.Double).Sortable(true);

            grid.Columns.Add(model => model.SettingDate).RenderedAs(model => model.SettingDate.ToShortDateString()).Filterable(GridFilterType.Double).Sortable(true);

            if (!isExport) grid.Columns.Add().RenderedAs(x => new HtmlString($"<a href={GetDeleteHref(x.Id)}>Удалить</a>"));

            return grid;
        }

        private IEnumerable<SelectListItem> GetProductOptions(IEnumerable<ProductPrice> productPrices)
        {
            Dictionary<int, SelectListItem> result = new Dictionary<int, SelectListItem>
            {
                { 0, new SelectListItem() }
            };

            foreach (var productPrice in productPrices)
            {
                if (!result.ContainsKey(productPrice.ProductId))
                {
                    result.Add(productPrice.ProductId, new SelectListItem() { Text = productPrice.Product.View, Value = productPrice.Product.View });
                }
            }

            return result.Values;
        }

        //private string GetEditHref(int id) => $"\"/ProductPrices/Edit/{id}?returnUrl=/ProductPrices/Query\"";

        private string GetDeleteHref(int id) => $"\"/ProductPrices/Delete/{id}?returnUrl=/ProductPrices/Query\"";

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
