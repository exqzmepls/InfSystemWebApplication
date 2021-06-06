using OfficeOpenXml;
using System;
using System.Web;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;

namespace InfSystemWebApplication.Models
{
    public class InfSystemDbInitializer : CreateDatabaseIfNotExists<InfSystemContext>
    {
        protected override void Seed(InfSystemContext context)
        {
            PositionsInitializer.Initialize(context);

            OKEIInitializer.Initialize(context);

            OKPDInitializer.Initialize(context);

            base.Seed(context);
        }
    }

    public abstract class PositionsInitializer
    {
        private static List<Position> _positions = new List<Position>
        {
            new Position { Id = (int)PositionId.ADMIN, Name = "Управляющий" },
            new Position { Id = (int)PositionId.ASSISTANT, Name = "Продавец" },
            new Position { Id = (int)PositionId.SENIOR_ASSISTANT, Name = "Старщий продавец" }
        };

        public static void Initialize(InfSystemContext context)
        {
            context.Positions.AddRange(_positions);
        }
    }

    public abstract class OKEIInitializer
    {
        private static readonly string _path = HttpContext.Current?.Server.MapPath("~/App_Data/okei.xlsx") ?? @"C:\Users\exqzme\Desktop\okei.xlsx"; //@"C:\Users\exqzme\Desktop\okei.xlsx"

        public static void Initialize(InfSystemContext context)
        {
            using (var pack = new ExcelPackage(new FileInfo(_path)))
            {
                ExcelWorksheet ws = pack.Workbook.Worksheets[0];

                int currentRow = 8;
                while (true)
                {
                    int id = int.Parse(ws.Cells[$"B{currentRow}"].Text);
                    string name = ws.Cells[$"C{currentRow}"].Text;
                    string shortName = ws.Cells[$"D{currentRow}"].Text;

                    if (string.IsNullOrEmpty(shortName) || string.IsNullOrWhiteSpace(shortName))
                    {
                        break;
                    }
                    else
                    {
                        context.Units.Add(new Unit { Id = id, Name = name, ShortName = shortName });
                        //try
                        //{
                        //    AddUnit(id, name, shortName);
                        //}
                        //catch (Exception ex)
                        //{
                        //    Console.WriteLine(ex.Message);
                        //}
                    }
                    currentRow++;
                }
            }
        }
    }

    public abstract class OKPDInitializer
    {
        private static readonly string _path = HttpContext.Current?.Server.MapPath("~/App_Data/okpd.xlsx") ?? @"C:\Users\exqzme\Desktop\okpd.xlsx"; //@"C:\Users\exqzme\Desktop\okpd.xlsx"

        public static void Initialize(InfSystemContext context)
        {
            using (var pack = new ExcelPackage(new FileInfo(_path)))
            {
                ExcelWorksheet ws = pack.Workbook.Worksheets[0];

                int iRow = 0;
                string[] sa;
                List<int> ids;

                //Console.WriteLine("Удаление...");
                //using (InfSystemContext db = new InfSystemContext())
                //{
                //    db.ProductClasses.RemoveRange(db.ProductClasses);
                //    db.SaveChanges();
                //}

                while (true)
                {
                    iRow++;

                    try
                    {
                        string s = ws.Cells[$"B{iRow}"].Text;
                        sa = ws.Cells[$"B{iRow}"].Text.Split('.', ',');
                        //Console.WriteLine(ws.Cells[$"B{iRow}"].Text);
                        ids = GetIds(ws.Cells[$"B{iRow}"].Text);
                    }
                    catch
                    {
                        continue;
                    }
                    if (ids[0] > 33)
                    {
                        break;
                    }
                    switch (ids.Count)
                    {
                        case 1:
                            AddClass(GetId(sa), ws.Cells[$"C{iRow}"].Text, context);
                            //try
                            //{
                            //    AddClass(GetId(sa), ws.Cells[$"C{iRow}"].Text);
                            //}
                            //catch (Exception ex)
                            //{
                            //    Console.WriteLine(ex.Message);
                            //}
                            break;

                        case 2:
                            if (sa[1].Length > 1)
                            {
                                AddGroup(GetId(sa), ws.Cells[$"C{iRow}"].Text, GetId(sa[0], sa[1].Substring(0, 1)), context);
                                //try
                                //{
                                //    AddGroup(GetId(sa), ws.Cells[$"C{iRow}"].Text, GetId(sa[0], sa[1].Substring(0, 1)));
                                //}
                                //catch (Exception ex)
                                //{
                                //    Console.WriteLine(ex.Message);
                                //}
                            }
                            else
                            {
                                AddSubClass(GetId(sa), ws.Cells[$"C{iRow}"].Text, GetId(sa[0]), context);
                                //try
                                //{
                                //    AddSubClass(GetId(sa), ws.Cells[$"C{iRow}"].Text, GetId(sa[0]));
                                //}
                                //catch (Exception ex)
                                //{
                                //    Console.WriteLine(ex.Message);
                                //}
                            }
                            break;

                        case 3:
                            if (sa[2].Length > 1)
                            {
                                AddKind(GetId(sa), ws.Cells[$"C{iRow}"].Text, GetId(sa[0], sa[1], sa[2].Substring(0, 1)), context);
                                //try
                                //{
                                //    AddKind(GetId(sa), ws.Cells[$"C{iRow}"].Text, GetId(sa[0], sa[1], sa[2].Substring(0, 1)));
                                //}
                                //catch (Exception ex)
                                //{
                                //    Console.WriteLine(ex.Message);
                                //}
                            }
                            else
                            {
                                AddSubGroup(GetId(sa), ws.Cells[$"C{iRow}"].Text, GetId(sa[0], sa[1]), context);
                                //try
                                //{
                                //    AddSubGroup(GetId(sa), ws.Cells[$"C{iRow}"].Text, GetId(sa[0], sa[1]));
                                //}
                                //catch (Exception ex)
                                //{
                                //    Console.WriteLine(ex.Message);
                                //}
                            }
                            break;

                        case 4:
                            if (ids[3] % 10 == 0)
                            {
                                AddCategory(GetId(sa), ws.Cells[$"C{iRow}"].Text, GetId(sa[0], sa[1], sa[2]), context);
                                //try
                                //{
                                //    AddCategory(GetId(sa), ws.Cells[$"C{iRow}"].Text, GetId(sa[0], sa[1], sa[2]));
                                //}
                                //catch (Exception ex)
                                //{
                                //    Console.WriteLine(ex.Message);
                                //}
                            }
                            break;
                    }
                }
            }
        }

        static void AddClass(int id, string name, InfSystemContext db)
        {
            ProductClass productClass = new ProductClass() { Id = id, Name = name };
            db.ProductClasses.Add(productClass);
        }

        static void AddSubClass(int id, string name, int classId, InfSystemContext db)
        {
            ProductSubclass productSubClass = new ProductSubclass() { Id = id, Name = name, ClassId = classId };
            db.ProductSubClasses.Add(productSubClass);
        }

        static void AddGroup(int id, string name, int subclassId, InfSystemContext db)
        {
            ProductGroup productGroup = new ProductGroup() { Id = id, Name = name, SubclassId = subclassId };
            db.ProductGroups.Add(productGroup);
        }

        static void AddSubGroup(int id, string name, int groupId, InfSystemContext db)
        {
            ProductSubgroup productSubGroup = new ProductSubgroup() { Id = id, Name = name, GroupId = groupId };
            db.ProductSubGroups.Add(productSubGroup);
        }

        static void AddKind(int id, string name, int subgroupId, InfSystemContext db)
        {
            ProductKind productKind = new ProductKind() { Id = id, Name = name, SubgroupId = subgroupId };
            db.ProductKinds.Add(productKind);
        }

        static void AddCategory(int id, string name, int kindId, InfSystemContext db)
        {
            ProductCategory productCategory = new ProductCategory() { Id = id, Name = name, KindId = kindId };
            db.ProductCategories.Add(productCategory);
        }

        static List<int> GetIds(string code)
        {
            List<int> result = new List<int>();
            foreach (var c in code.Split('.', ','))
            {
                result.Add(int.Parse(c));
            }
            return result;
        }

        static int GetId(params string[] ids)
        {
            string result = "";
            foreach (var i in ids)
            {
                result += i;
            }
            return int.Parse(result);
        }
    }
}