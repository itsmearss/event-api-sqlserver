﻿namespace TestProjectAnnur.Data.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<Event> Events { get; } = new List<Event>();
    }
}
