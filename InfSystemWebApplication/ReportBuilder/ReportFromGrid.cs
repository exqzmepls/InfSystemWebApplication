using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Linq;

namespace InfSystemWebApplication.ReportBuilder
{
    public abstract class ReportFromGrid
    {
        const int _headerRowIndex = 1;
        const int _startRowIndex = _headerRowIndex + 1;
        const int _startColIndex = 1;

        public static byte[] Create<T>(string name, IGrid<T> grid)
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add(name);
                ws.Select();

                PrintHeaders(ws, grid);

                PrintData(ws, grid);

                PrintTotal(ws, grid);

                PrintSums(ws, grid);

                ExcelAddress field = new ExcelAddress(_headerRowIndex, _startColIndex, _headerRowIndex + grid.Rows.Count(), grid.Columns.Count());
                ws.Cells[field.Address].AutoFitColumns();
                ws.Select(field);

                return package.GetAsByteArray();
            }
        }

        private static void PrintHeaders<T>(ExcelWorksheet sheet, IGrid<T> grid)
        {
            int colIndex = _startColIndex;

            foreach (var column in grid.Columns)
            {
                ExcelRange cell = sheet.Cells[_headerRowIndex, colIndex++];

                cell.Value = column.Title;
                cell.Style.Font.Bold = true;
                cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                //column.IsEncoded = false;
            }
        }

        private static void PrintData<T>(ExcelWorksheet sheet, IGrid<T> grid)
        {
            int rowIndex = _startRowIndex;

            foreach (var gridRow in grid.Rows)
            {
                int colIndex = _startColIndex;

                foreach (var column in grid.Columns) sheet.Cells[rowIndex, colIndex++].Value = column.ValueFor(gridRow as IGridRow<object>).ToString();

                rowIndex++;
            }
        }

        private static void PrintTotal<T>(ExcelWorksheet sheet, IGrid<T> grid)
        {
            int rowIndex = _headerRowIndex + grid.Rows.Count(), colIndex = _startColIndex + grid.Columns.Count() + 1;

            sheet.Cells[rowIndex, colIndex].Value = "Всего:";
            sheet.Cells[rowIndex, colIndex + 1].Value = grid.Rows.Count();

            ExcelAddress field = new ExcelAddress(rowIndex, colIndex, rowIndex, colIndex + 1);
            sheet.Cells[field.Address].AutoFitColumns();
        }

        private static void PrintSums<T>(ExcelWorksheet sheet, IGrid<T> grid)
        {
            int colIndex = _startColIndex, rowIndex = _startRowIndex + grid.Rows.Count() + 1;

            foreach (var column in grid.Columns)
            {
                if (column is IGridColumn<T, double?> || column is IGridColumn<T, double>)
                {
                    sheet.Cells[rowIndex, colIndex].Value = Sum(grid, column);                    
                }

                colIndex++;
            }
        }

        private static double Sum<T>(IGrid<T> grid, IGridColumn column)
        {
            double sum = 0;

            foreach (IGridRow<object> row in grid.Rows)
            {
                string s = column.ValueFor(row).ToString();
                if (string.IsNullOrEmpty(s)) s = "0";
                sum += Convert.ToDouble(s);
            }

            return sum;
        }
    }
}