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
    public class SuppliersController : Controller
    {
        private IRepository db;

        string reportName = "Suppliers";

        public SuppliersController() { db = new Repository(); }

        public SuppliersController(IRepository repository) { db = repository; }

        // GET: Suppliers
        [Authorize(Roles = "user")]
        public ActionResult Index()
        {
            return View(db.GetList<Supplier>());
        }

        // GET: Sales/Details/5
        [Authorize(Roles = "user")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Find<Supplier>(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View("Details", supplier);
        }

        // GET: Suppliers/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Suppliers/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Create([Bind(Include = "Id,Name,PhoneNumber")] Supplier supplier, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                db.Add(supplier);
                db.SaveChanges();
                return Redirect(returnUrl ?? "/Home/Index");
                //return RedirectToAction("Index");
            }

            return View("Create", supplier);
        }

        // GET: Suppliers/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Find<Supplier>(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Edit([Bind(Include = "Id,Name,PhoneNumber")] Supplier supplier, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                db.Update(supplier);
                db.SaveChanges();
                return Redirect(returnUrl ?? "/Home/Index");
                //return RedirectToAction("Index");
            }
            return View("Edit", supplier);
        }

        // GET: Suppliers/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Find<Supplier>(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteConfirmed(int id, string returnUrl)
        {
            Supplier supplier = db.Find<Supplier>(id);
            db.Remove(supplier);
            db.SaveChanges();
            return Redirect(returnUrl ?? "/Home/Index");
            //return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        public ActionResult Download()
        {
            return File(Report.Create(reportName, db.GetList<Supplier>(), EntityProperty.GetProperties(typeof(Supplier))), "application/unknown", reportName + "-Report.xlsx");

            //return Report("Suppliers", GetAvailable(), db.Suppliers.ToList());
        }

        //private FileContentResult Report<T>(string name, List<EntityPropertyViewModel> entityProperties, List<T> data)
        //{

        //    using (MemoryStream stream = new MemoryStream())
        //    {
        //        Report report = new Report(name, stream, entityProperties, data, typeof(T));
        //        report.Create();

        //        stream.Flush();

        //        return new FileContentResult(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileDownloadName = $"{name}-report.xlsx" };
        //    }
        //}

        [HttpGet]
        [Authorize(Roles = "user")]
        public ViewResult Query(/*string queryString = null*/)
        {
            return View("Query", CreateGrid());
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        public ActionResult Export(/*string queryString*/)
        {
            return File(ReportFromGrid.Create(reportName, CreateGrid(true)), "application/unknown", reportName + "-Report.xlsx");

            // Using EPPlus from nuget
            //using (ExcelPackage package = new ExcelPackage())
            //{
            //    Int32 row = 2;
            //    Int32 col = 1;

            //    package.Workbook.Worksheets.Add("Data");
            //    IGrid<Supplier> grid = CreateGrid(true);
            //    ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

            //    foreach (IGridColumn column in grid.Columns)
            //    {
            //        sheet.Cells[1, col].Value = column.Title;
            //        sheet.Cells[1, col].Style.Font.Bold = true;
            //        sheet.Column(col++).AutoFit();

            //        column.IsEncoded = false;
            //    }

            //    foreach (IGridRow<Supplier> gridRow in grid.Rows)
            //    {
            //        col = 1;
            //        foreach (IGridColumn column in grid.Columns)
            //            sheet.Cells[row, col++].Value = column.ValueFor(gridRow);

            //        row++;
            //    }

            //    return File(package.GetAsByteArray(), "application/unknown", "Report.xlsx");
            //}
        }

        private IGrid<Supplier> CreateGrid(/*string queryString,*/ bool isExport = false)
        {
            IGrid<Supplier> grid = new Grid<Supplier>(db.GetList<Supplier>())
            {
                EmptyText = "Нет данных"
            };

            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request?.QueryString;


            grid.Columns.Add(model => model.Id).Filterable(GridFilterType.Double).Sortable(true);
            grid.Columns.Add(model => model.Name).Filterable(GridFilterType.Double).Sortable(true);
            grid.Columns.Add(model => model.PhoneNumber).Filterable(GridFilterType.Double).Sortable(true);

            //if (queryString != null)
            //{
            //    string[] p = new string[2];
            //    foreach (var par in queryString.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
            //    {
            //        p = par.Split('=');
            //        grid.Query.Add(p[0], p[1]);
            //    }
            //}

            if (!isExport)
            {
                grid.Columns.Add().RenderedAs(x => new HtmlString($"<a href={GetEditHref(x.Id)}>Изменить</a> | <a href={GetDeleteHref(x.Id)}>Удалить</a>"));
            }

            return grid;
        }

        private string GetEditHref(int id) => $"\"/Suppliers/Edit/{id}?returnUrl=/Suppliers/Query\"";

        private string GetDeleteHref(int id) => $"\"/Suppliers/Delete/{id}?returnUrl=/Suppliers/Query\"";

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
