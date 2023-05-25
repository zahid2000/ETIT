using System.ComponentModel.DataAnnotations;

namespace ETIT.Models
{
    // Models/Company.cs
    public class Company
    {
        public Company()
        {
            Products = new List<Product>();
        }
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
        [Required, MaxLength(255)]
        public string Address { get; set; }
        public string Phone { get; set; }
        public virtual List<Product> Products { get; set; }
    }
}