using OfficeOpenXml.Attributes;
using OfficeOpenXml.Table;
using System.ComponentModel.DataAnnotations.Schema;
using TestProjectAnnur.Data.Models;

namespace TestProjectAnnur.Data.DTOs
{
    public class ReportDTO
    {
    }

    [EpplusTable(TableStyle = TableStyles.Light8, PrintHeaders = true, AutofitColumns = true, AutoCalculate = false, ShowTotal = false, ShowFirstColumn = false)]
    public class ExportEvent
    {
        [EpplusTableColumn(Order = 0, Header = "ID")]
        public int Id { get; set; }

        [EpplusTableColumn(Order = 1, Header = "Title")]
        public string Title { get; set; }

        [EpplusTableColumn(Order = 2, Header = "Description")]
        public string Description { get; set; }

        [EpplusTableColumn(Order = 3, Header = "Category")]
        public int? CategoryId { get; set; }

        [EpplusTableColumn(Order = 4, Header = "Date", NumberFormat = "yyyy-MM-dd")]
        public DateOnly Date { get; set; }

        [EpplusTableColumn(Order = 5, Header = "Time")]
        public TimeSpan Time { get; set; }

        [EpplusTableColumn(Order = 6, Header = "Location")]
        public string Location { get; set; }

        [EpplusTableColumn(Order = 7, Header = "Max Attendees")]
        public int MaxAttendees { get; set; }

        [EpplusTableColumn(Order = 8, Header = "Status")]
        public Status Status { get; set; }

        [EpplusTableColumn(Order = 9, Header = "Flyer Filename")]
        public string? Flyer { get; set; }

        [EpplusTableColumn(Order = 10, Header = "Cover Filename")]
        public string? Cover { get; set; }

        [EpplusTableColumn(Order = 11, Header = "Created At", NumberFormat = "yyyy-MM-dd")]
        public DateTime CreatedAt { get; set; }

        [EpplusTableColumn(Order = 12, Header = "Updated At", NumberFormat = "yyyy-MM-dd")]
        public DateTime? UpdatedAt { get; set; }

    }
}
