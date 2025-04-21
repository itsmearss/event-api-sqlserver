using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Drawing.Text;
using TestProjectAnnur.Data.Models;

namespace TestProjectAnnur.Services
{
    public class ProductionScheduleService
    {
        private readonly DataAccess _dataAccess;

        public ProductionScheduleService(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public void GenerateReport(string factoryId, string outputPath, int year, int month)
        {
            // Calculate start and end dates for the month
            DateOnly startDate = new DateOnly(year, month, 1);
            DateOnly endDate = new DateOnly(year, month, DateTime.DaysInMonth(year, month));

            // Get data from database using EF
            var stitchingData = _dataAccess.GetStitchingData(factoryId, startDate, endDate);
            var assemblyData = _dataAccess.GetAssemblyData(factoryId, startDate, endDate);
            var nonWorkingDates = _dataAccess.GetNonWorkingDates(factoryId);

            using (ExcelPackage package = new ExcelPackage())
            {
                // Create the worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add($"{year}-{month:00} Production Planning");

                // Set the report title
                worksheet.Cells[1, 1].Value = $"{year}-{month:00} Production Planning";
                worksheet.Cells[1, 1, 1, 2].Merge = true;
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.Font.Size = 14;

                // Create date header row
                worksheet.Cells[2, 1].Value = "Date";

                int daysInMonth = DateTime.DaysInMonth(year, month);
                for (int day = 1; day <= daysInMonth; day++)
                {
                    DateOnly date = new DateOnly(year, month, day);
                    worksheet.Cells[2, day + 1].Value = date.ToString("M/d");
                }

                // Add summary columns
                worksheet.Cells[2, daysInMonth + 2].Value = "Total Stop";
                worksheet.Cells[2, daysInMonth + 3].Value = "Original WD";
                worksheet.Cells[2, daysInMonth + 4].Value = "Actual WD";

                // Format header row
                using (var range = worksheet.Cells[2, 1, 2, daysInMonth + 4])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                }

                // Create Team and Section rows
                int currentRow = 3;

                // Get unique team codes from both stitching and assembly data
                HashSet<string> teams = new HashSet<string>();
                foreach (var item in stitchingData)
                {
                    teams.Add(item.ST_Team);
                }
                foreach (var item in assemblyData)
                {
                    teams.Add(item.AS_Team);
                }

                // Sort team codes
                List<string> sortedTeams = teams.OrderBy(p => p).ToList();

                foreach (string team in sortedTeams)
                {
                    // Add Stitching row
                    currentRow = AddSectionRow(worksheet, factoryId, team, "Stitching", startDate, daysInMonth,
                                               stitchingData, nonWorkingDates, "3", currentRow);

                    // Add Assembly row
                    currentRow = AddSectionRow(worksheet, factoryId, team, "Assembly", startDate, daysInMonth,
                                               assemblyData, nonWorkingDates, "5", currentRow);
                }

                // Auto-fit columns
                for (int col = 1; col <= daysInMonth + 4; col++)
                {
                    worksheet.Column(col).AutoFit();
                }

                // Save the file
                package.SaveAs(new FileInfo(outputPath));
            }

        
        }

        private int AddSectionRow(ExcelWorksheet worksheet, string factoryId, string team, string sectionCode, DateOnly startDate, int daysInMonth,
                                   object dataList, List<A_T_NonWorking_Date> nonWorkingDates,
                                   string sectionDbCode, int rowIndex)
        {
            // Add Team Code and Section
            worksheet.Cells[rowIndex, 1].Value = $"{team} {sectionCode}";

            // Create a dictionary to store the model per day
            Dictionary<int, string> modelsByDay = new Dictionary<int, string>();

            // Process based on section type
            if (sectionCode == "Stitching")
            {
                var stitchingList = (List<Z_Production_Schedule>)dataList;
                modelsByDay = ProcessStitchingData(factoryId, team, startDate, daysInMonth, stitchingList, nonWorkingDates, sectionDbCode);
            }
            else // Assembly
            {
                var assemblyList = (List<Z_Production_Schedule>)dataList;
                modelsByDay = ProcessAssemblyData(factoryId, team, startDate, daysInMonth, assemblyList, nonWorkingDates, sectionDbCode);
            }

            // Fill in the worksheet with model names
            int totalStop = 0;
            for (int day = 1; day <= daysInMonth; day++)
            {
                if (modelsByDay.TryGetValue(day, out string modelName))
                {
                    worksheet.Cells[rowIndex, day + 1].Value = modelName;

                    // Apply color based on model name
                    if (modelName == "no order")
                    {
                        totalStop++;
                    }
                    else
                    {
                        // For actual models, apply a green background
                        worksheet.Cells[rowIndex, day + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[rowIndex, day + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                    }
                }
                else
                {
                    worksheet.Cells[rowIndex, day + 1].Value = "no order";
                    totalStop++;
                }
            }

            // Add summary columns
            worksheet.Cells[rowIndex, daysInMonth + 2].Value = totalStop;
            worksheet.Cells[rowIndex, daysInMonth + 3].Value = daysInMonth;
            worksheet.Cells[rowIndex, daysInMonth + 4].Value = daysInMonth - totalStop;

            return rowIndex + 1;
        }

        private Dictionary<int, string> ProcessStitchingData(string factoryId, string team, DateOnly startDate, int daysInMonth,
                                             List<Z_Production_Schedule> stitchingList, List<A_T_NonWorking_Date> nonWorkingDates,
                                             string sectionDbCode)
        {
            Dictionary<int, string> modelsByDay = new Dictionary<int, string>();

            // Filter data for this team
            var teamData = stitchingList.Where(s => s.ST_Team == team).ToList();

            // Process each day of the month
            for (int day = 1; day <= daysInMonth; day++)
            {
                DateOnly currentDate = new DateOnly(startDate.Year, startDate.Month, day);

                // Get data for this date
                var dateData = teamData.Where(s => s.ST_Start_Date == currentDate).ToList();

                if (dateData.Any())
                {
                    // Group by model name and sum plan quantity
                    var modelGroups = dateData
                        .GroupBy(s => s.Model_Name)
                        .Select(g => new
                        {
                            ModelName = g.Key,
                            TotalQty = g.Sum(s => s.Plan_Qty),
                            MaxSequence = g.Max(s => s.UA_sequence)
                        })
                        .OrderByDescending(g => g.TotalQty)
                        .ThenByDescending(g => g.MaxSequence)
                        .ToList();

                    if (modelGroups.Any())
                    {
                        // Use the model with the biggest total quantity
                        modelsByDay[day] = modelGroups.First().ModelName;
                    }
                }
                else
                {
                    // Check if this is a non-working date
                    bool isNonWorkingDate = nonWorkingDates
                        .Any(n => n.Data_Date == currentDate &&
                                 n.Factory_ID == factoryId &&
                                 n.Section_Code == sectionDbCode &&
                                 n.PU_Code == team);

                    if (!isNonWorkingDate)
                    {
                        // Use the model from the previous working day
                        string lastModel = GetLastStitchingModelBeforeDate(teamData, currentDate);
                        if (!string.IsNullOrEmpty(lastModel))
                        {
                            modelsByDay[day] = lastModel;
                        }
                        else
                        {
                            modelsByDay[day] = "no order";
                        }
                    }
                    else
                    {
                        // This is a non-working day
                        modelsByDay[day] = "no order";
                    }
                }
            }

            return modelsByDay;
        }

        private Dictionary<int, string> ProcessAssemblyData(string factoryId, string team, DateOnly startDate, int daysInMonth,
                                             List<Z_Production_Schedule> assemblyList, List<A_T_NonWorking_Date> nonWorkingDates,
                                             string sectionDbCode)
        {
            Dictionary<int, string> modelsByDay = new Dictionary<int, string>();

            // Filter data for this team
            var teamData = assemblyList.Where(a => a.AS_Team == team).ToList();

            // Process each day of the month
            for (int day = 1; day <= daysInMonth; day++)
            {
                DateOnly currentDate = new DateOnly(startDate.Year, startDate.Month, day);

                // Get data for this date
                var dateData = teamData.Where(a => a.AS_Start_Date == currentDate).ToList();

                if (dateData.Any())
                {
                    // Group by model name and sum plan quantity
                    var modelGroups = dateData
                        .GroupBy(a => a.Model_Name)
                        .Select(g => new
                        {
                            ModelName = g.Key,
                            TotalQty = g.Sum(a => a.Plan_Qty),
                            MaxSequence = g.Max(a => a.FA_sequence)
                        })
                        .OrderByDescending(g => g.TotalQty)
                        .ThenByDescending(g => g.MaxSequence)
                        .ToList();

                    if (modelGroups.Any())
                    {
                        // Use the model with the biggest total quantity
                        modelsByDay[day] = modelGroups.First().ModelName;
                    }
                }
                else
                {
                    // Check if this is a non-working date
                    bool isNonWorkingDate = nonWorkingDates
                        .Any(n => n.Data_Date == currentDate &&
                                 n.Factory_ID == factoryId &&
                                 n.Section_Code == sectionDbCode &&
                                 n.PU_Code == team);

                    if (!isNonWorkingDate)
                    {
                        // Use the model from the previous working day
                        string lastModel = GetLastAssemblyModelBeforeDate(teamData, currentDate);
                        if (!string.IsNullOrEmpty(lastModel))
                        {
                            modelsByDay[day] = lastModel;
                        }
                        else
                        {
                            modelsByDay[day] = "no order";
                        }
                    }
                    else
                    {
                        // This is a non-working day
                        modelsByDay[day] = "no order";
                    }
                }
            }

            return modelsByDay;
        }

        private string GetLastStitchingModelBeforeDate(List<Z_Production_Schedule> data, DateOnly currentDate)
        {
            var previousData = data
                .Where(s => s.ST_Start_Date < currentDate)
                .OrderByDescending(s => s.ST_Start_Date)
                .ThenByDescending(s => s.UA_sequence)
                .FirstOrDefault();

            return previousData?.Model_Name;
        }

        private string GetLastAssemblyModelBeforeDate(List<Z_Production_Schedule> data, DateOnly currentDate)
        {
            var previousData = data
                .Where(a => a.AS_Start_Date < currentDate)
                .OrderByDescending(a => a.AS_Start_Date)
                .ThenByDescending(a => a.FA_sequence)
                .FirstOrDefault();

            return previousData?.Model_Name;
        }
    }
}
