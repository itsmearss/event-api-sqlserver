using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using TestProjectAnnur.Data.DTOs;
using TestProjectAnnur.Services;
using System.Globalization;
using System.Drawing;
using TestProjectAnnur.Data;
using Microsoft.EntityFrameworkCore;

namespace TestProjectAnnur.Controllers
{
    [ApiController]
    [Route("api/prodplan")]
    public class ProductionPlanController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductionScheduleService _productionScheduleService;

        public ProductionPlanController(ApplicationDbContext context, ProductionScheduleService productionScheduleService)
        {
            _context = context;
            _productionScheduleService = productionScheduleService;
        }

        [HttpGet("new-export-sched")]
        public ActionResult Index()
        {
            try
            {
                // Configuration
                string factoryId = "your_factory_id"; // Replace with your factory ID
                string outputPath = "ProductionPlanning.xlsx";

                _productionScheduleService.GenerateReport("2010", outputPath, 2023, 11);
                return Ok(outputPath);

            } catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500, $"Internal server error");

            }

        }

        [HttpGet("export-prod-plan")]
        public async Task<IActionResult> GetExportProductionPlan()
        {
            // Misalnya input string dari user
            string period = "202404";

            string Factory_ID = "2010";

            // Parse tahun dan bulan
            int year = int.Parse(period.Substring(0, 4));
            int month = int.Parse(period.Substring(4, 2));

            // Awal bulan
            var startDate = new DateOnly(year, month, 1);

            // Akhir bulan → ambil jumlah hari dalam bulan tsb
            var endDate = new DateOnly(year, month, DateTime.DaysInMonth(year, month));

            // Lanjut buat range tanggal
            var allDates = Enumerable.Range(0, endDate.DayNumber - startDate.DayNumber + 1)
                .Select(offset => startDate.AddDays(offset))
                .ToList();

            var planSt = await _context.Z_Production_Schedule
                .Where(p => p.ST_Start_Date >= startDate  && p.ST_Start_Date <= endDate)
                .ToListAsync();

            var nonWorkingDays = await _context.Z_Production_Schedule
                .Where(p => p.Factory_ID == Factory_ID)
                .Select(p => p.Factory_ID)
                .ToListAsync();

            var stream = new MemoryStream();
            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var ws = package.Workbook.Worksheets.Add("Production Schedule");
                    ws.Cells[1, 1].Value = " Production Planning";
                    ws.Cells[1, 1].Style.Font.Bold = true;
                    // Header
                    ws.Cells[2, 1].Value = "date";

                    ws.Cells[3, 1, 3, 50].AutoFilter = true;

                    var culture = new CultureInfo("zh-CN");

                    int col = 3;
                    foreach (var d in allDates)
                    {
                        var cell = ws.Cells[2, col];
                        cell.Value = d;
                        cell.Style.Numberformat.Format = "M/d";

                        var dayCell = ws.Cells[3, col];
                        dayCell.Value = d.ToString("dddd", culture);
                        dayCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        dayCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        ws.Cells[4, col].Value = "Newwww";

                        col++;
                    }

                    ws.Cells[2, col++].Value = "Total Stop";
                    ws.Cells[2, col++].Value = "Original WD";
                    ws.Cells[2, col++].Value = "Actual WD";

                    await package.SaveAsync();
                }

                stream.Position = 0;
                string excelName = "$download.xlsx";
                return File(stream, "application/xlsx", excelName);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                var inex = ex.InnerException?.Message ?? "⚠";
                return StatusCode(500, $"Internal server error: {message} {inex}");
            }
        }
    }
}