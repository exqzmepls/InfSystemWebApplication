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
    public class SoldProductsController : Controller
    {
        private IRepository db;

        string reportName = "SoldProducts";

        public SoldProductsController() { db = new Repository(); }

        public SoldProductsController(IRepository repository) { db = repository; }

        // GET: SoldProducts
        [Authorize(Roles = "user")]
        public ActionResult Index()
        {
            var soldProducts = db.GetList<SoldProduct>();
            return View(soldProducts);
        }

        //// GET: SoldProducts/Details/5
        //public ActionResult Details(int? id1, int? id2)
        //{
        //    if (id1 == null || id2 == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    SoldProduct soldProduct = db.SoldProducts.Find(id1, id2);
        //    if (soldProduct == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(soldProduct);
        //}

        //// GET: SoldProducts/Create
        //public ActionResult Create()
        //{
        //    ViewBag.ProductPriceId = new SelectList(db.ProductPrices, "Id", "View");
        //    ViewBag.SaleId = new SelectList(db.Sales, "Id", "View");
        //    return View();
        //}

        //// POST: SoldProducts/Create
        //// Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        //// сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "SaleId,ProductPriceId,Amount")] SoldProduct soldProduct)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.SoldProducts.Add(soldProduct);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.ProductPriceId = new SelectList(db.ProductPrices, "Id", "View", soldProduct.ProductPriceId);
        //    ViewBag.SaleId = new SelectList(db.Sales, "Id", "View", soldProduct.SaleId);
        //    return View(soldProduct);
        //}

        //// GET: SoldProducts/Edit/5...
        //public ActionResult Edit(int? id1, int? id2)
        //{
        //    if (id1 == null || id2 == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    SoldProduct soldProduct = db.SoldProducts.Find(id1, id2);
        //    if (soldProduct == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.ProductPriceId = new SelectList(db.ProductPrices, "Id", "View", soldProduct.ProductPriceId);
        //    ViewBag.SaleId = new SelectList(db.Sales, "Id", "View", soldProduct.SaleId);
        //    return View(soldProduct);
        //}

        //// POST: SoldProducts/Edit/5
        //// Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        //// сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "SaleId,ProductPriceId,Amount")] SoldProduct soldProduct)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(soldProduct).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.ProductPriceId = new SelectList(db.ProductPrices, "Id", "View", soldProduct.ProductPriceId);
        //    ViewBag.SaleId = new SelectList(db.Sales, "Id", "View", soldProduct.SaleId);
        //    return View(soldProduct);
        //}

        //// GET: SoldProducts/Delete/5
        //public ActionResult Delete(int? id1, int? id2)
        //{
        //    if (id1 == null || id2 == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    SoldProduct soldProduct = db.SoldProducts.Find(id1, id2);
        //    if (soldProduct == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(soldProduct);
        //}

        //// POST: SoldProducts/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int? id1, int? id2)
        //{
        //    SoldProduct soldProduct = db.SoldProducts.Find(id1, id2);
        //    db.SoldProducts.Remove(soldProduct);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        [HttpGet]
        [Authorize(Roles = "user")]
        public ActionResult Download()
        {
            return File(Report.Create(reportName, db.GetList<SoldProduct>(), EntityProperty.GetProperties(typeof(SoldProduct))), "application/unknown", reportName + "-Report.xlsx");
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

        private IGrid<SoldProduct> CreateGrid(bool isExport = false)
        {
            var soldProducts = db.GetList<SoldProduct>();

            IGrid<SoldProduct> grid = new Grid<SoldProduct>(soldProducts)
            {
                ViewContext = new ViewContext { HttpContext = HttpContext },
                Query = Request?.QueryString,
                EmptyText = "Нет данных"
            };

            grid.Columns.Add(model => model.Sale.View)
                .UsingFilterOptions(GetSaleOptions(soldProducts))
                .Filterable(GridFilterType.Double)
                .Sortable(true);

            grid.Columns.Add(model => model.ProductPrice.ProductView)
                .UsingFilterOptions(GetProductOptions(soldProducts))
                .Filterable(GridFilterType.Double)
                .Sortable(true);

            grid.Columns.Add(model => model.ProductPrice.ValuePerOneUnit)
                .Filterable(GridFilterType.Double)
                .Sortable(true);

            grid.Columns.Add(model => model.Amount)
                .Filterable(GridFilterType.Double)
                .Sortable(true);

            grid.Columns.Add(model => model.Total)
                .Filterable(GridFilterType.Double)
                .Sortable(true);

            return grid;
        }

        #region filter options
        private IEnumerable<SelectListItem> GetSaleOptions(IEnumerable<SoldProduct> soldProducts)
        {
            Dictionary<int, SelectListItem> result = new Dictionary<int, SelectListItem>
            {
                { 0, new SelectListItem() }
            };

            foreach (var soldProduct in soldProducts)
            {
                if (!result.ContainsKey(soldProduct.SaleId))
                {
                    result.Add(soldProduct.SaleId, new SelectListItem() { Text = soldProduct.Sale.View, Value = soldProduct.Sale.View });
                }
            }   

            return result.Values;
        }

        private IEnumerable<SelectListItem> GetProductOptions(IEnumerable<SoldProduct> soldProducts)
        {
            Dictionary<int, SelectListItem> result = new Dictionary<int, SelectListItem>
            {
                { 0, new SelectListItem() }
            };

            foreach (var soldProduct in soldProducts)
            {
                if (!result.ContainsKey(soldProduct.ProductPriceId))
                {
                    result.Add(soldProduct.ProductPriceId, new SelectListItem() { Text = soldProduct.ProductPrice.ProductView, Value = soldProduct.ProductPrice.ProductView });
                }
            }

            return result.Values;
        }
        #endregion

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
