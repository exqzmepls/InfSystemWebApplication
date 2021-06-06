using InfSystemWebApplication.Models;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace InfSystemWebApplication.ReportBuilder
{
    public abstract class Report
    {
        //private MemoryStream _stream;
        //private List<EntityPropertyViewModel> _properties;
        //private IList _entities;
        //private Type _entityType;
        //private string _reportName;

        const int _headerRowIndex = 1;
        const int _startRowIndex = _headerRowIndex + 1;
        const int _startColIndex = 1;

        //public Report(string name, MemoryStream stream, List<EntityPropertyViewModel> properties, IList entities, Type entityType)
        //{
        //    _reportName = name;
        //    _stream = stream;
        //    _properties = properties;
        //    _entities = entities;
        //    _entityType = entityType;
        //}

        public static byte[] Create<T>(string name, List<T> data, List<EntityProperty> entityProperties)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add(name);
                ws.Select();

                PrintHeaders(ws, entityProperties);

                PrintData(ws, data, entityProperties);

                ExcelAddress field = new ExcelAddress(_headerRowIndex, _startColIndex, _headerRowIndex + data.Count, entityProperties.Count);
                ws.Cells[field.Address].AutoFitColumns();
                ws.Select(field);

                return package.GetAsByteArray();
            }
        }

        //public void Create()
        //{
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        //    using (ExcelPackage package = new ExcelPackage(_stream))
        //    {
        //        ExcelWorksheet ws = package.Workbook.Worksheets.Add(_reportName);
        //        ws.Select();

        //        PrintHeaders(ws);

        //        PrintData(ws);

        //        ExcelAddress field = new ExcelAddress(_headerRowIndex, 1, _headerRowIndex + _entities.Count, _properties.Count);
        //        ws.Cells[field.Address].AutoFitColumns();
        //        ws.Select(field);

        //        package.Save();
        //    }
        //}

        private static void PrintHeaders(ExcelWorksheet sheet, List<EntityProperty> entityProperties)
        {
            int colIndex = _startColIndex;
            foreach (var property in entityProperties)
            {
                ExcelRange cell = sheet.Cells[_headerRowIndex, colIndex++];
                cell.Value = property.DisplayName;
                cell.Style.Font.Bold = true;
                cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            }
        }

        private static void PrintData<T>(ExcelWorksheet sheet, List<T> data, List<EntityProperty> entityProperties)
        {
            int rowIndex = _startRowIndex;

            Type entityType = typeof(T);

            foreach (var obj in data)
            {
                int colIndex = _startColIndex;
                foreach (var property in entityProperties)
                {
                    PropertyInfo propertyInfo = entityType.GetProperty(property.Name);
                    sheet.Cells[rowIndex, colIndex++].Value = propertyInfo.GetValue(obj)?.ToString();
                }
                rowIndex++;
            }
        }
    }
}