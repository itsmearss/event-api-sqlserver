namespace TestProjectAnnur.Data.DTOs
{
    public class CategoryDTO
    {
        public string Name { get; set; }
    }

    public class CategoryResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
