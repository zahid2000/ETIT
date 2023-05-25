using ETIT.Models.Auth;
using ETIT.Utilities.Enums;
using System.ComponentModel.DataAnnotations;

namespace ETIT.Models
{
    // Models/Delivery.cs
    public class Delivery
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int CourierId { get; set; }
        [Required]
        public DeliveryStatus Status { get; set; }
        public DateTime Date { get; set; }
        public Order? Order { get; set; }
        public AppUser? AppUser { get; set; }
    }
}