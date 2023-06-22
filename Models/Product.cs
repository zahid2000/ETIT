namespace ETIT.Models
{
    // Models/Product.cs
    public class Product
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }
        public bool IsDeleted { get; set; }
        public Company? Company { get; set; }
        public Category? Category { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public Product()
        {
            ProductImages = new List<ProductImage>();
        }
    }
}