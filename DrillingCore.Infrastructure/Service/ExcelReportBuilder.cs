using ClosedXML.Excel;
using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;

namespace DrillingCore.Infrastructure.Service
{
    public class ExcelReportBuilder : IExcelReportBuilder
    {
        public byte[] BuildTimesheetExcel(User user, DateOnly fromDate, DateOnly toDate, List<TimesheetDayDto> data)
        {
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Timesheet");

            // ✨ Title block
            sheet.Range("B1:F5").Merge().Value = "Time Sheet";
            sheet.Range("B1:F5").Style
                .Font.SetBold().Font.SetFontSize(28).Font.SetFontName("Calibri")
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            // 🧾 Employee Info
            sheet.Range("B6:F6").Merge().Value = $"Employee Name: {user.FullName}";
            sheet.Range("B7:F7").Merge().Value = $"Pay Period: {fromDate:MMMM yyyy}";
            sheet.Range("B6:F7").Style.Font.SetFontSize(12);
            sheet.Row(6).Height = 30;
            sheet.Row(7).Height = 30;

            // 💸 Cash Advance block
            sheet.Range("G4:K5").Merge().Value = "Cash Advance for this Month:";
            sheet.Range("G4:K5").Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                .Font.SetBold()
                .Border.OutsideBorder = XLBorderStyleValues.Thin;

            // 📊 Table Headers (starting from row 9)
            var headers = new[]
            {
        "Maint($___/Hr)", "Meter($___/M)", "Travel($__/Hrs)", "Day Rate",
        "Standby", "Truck", "ATV", "Hot Shot", "Job Name", "Narrative of daily events"
    };

            var headerRow = sheet.Row(9);
            headerRow.Cell(1).Value = "Day"; // A
            for (int i = 0; i < headers.Length; i++)
            {
                headerRow.Cell(i + 2).Value = headers[i];
                headerRow.Cell(i + 2).Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRow.Cell(i + 2).Style.Font.SetBold();
                headerRow.Cell(i + 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            // ☑ Data rows
            int row = 10;
            for (int i = 0; i < data.Count; i++)
            {
                var day = data[i];
                var meterAmount = day.MeterRate * (decimal)day.Meters;

                var currentRow = sheet.Row(row);
                currentRow.Cell(1).Value = i + 1; // Day number (A)
                currentRow.Cell(2).Value = 0;     // Maint
                currentRow.Cell(3).Value = meterAmount;
                currentRow.Cell(4).Value = 0;     // Travel
                currentRow.Cell(5).Value = day.DayRate;
                currentRow.Cell(6).Value = 0;     // Standby
                currentRow.Cell(7).Value = 0;     // Truck
                currentRow.Cell(8).Value = 0;     // ATV
                currentRow.Cell(9).Value = 0;     // Hot Shot
                currentRow.Cell(10).Value = user.JobTitle;
                currentRow.Cell(11).Value = day.Narrative;

                // Narrative → merge columns K, L, M
                sheet.Range(row, 11, row, 13).Merge();
                row++;
            }

            // 🧮 Totals row with formulas
            var totalRow = sheet.Row(row);
            totalRow.Cell(1).Value = "TOTAL"; // A
            for (int col = 2; col <= 10; col++) // B to J
            {
                var colLetter = XLHelper.GetColumnLetterFromNumber(col);
                totalRow.Cell(col).FormulaA1 = $"SUM({colLetter}10:{colLetter}{row - 1})";
            }

            totalRow.Style
                .Font.SetBold()
                .Fill.BackgroundColor = XLColor.Gray;

            // 🪟 Adjustments
            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

    }
}
