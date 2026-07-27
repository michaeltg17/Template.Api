namespace Domain.Models
{
    public class Product : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageName { get; set; }
        public Uri? ImageUrl { get; set; }
    }
}