﻿using ETIT.Models.Auth;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ETIT.Models
{
    // Models/Order.cs
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal Price { get; set; }
        public DateTime CreatedDate { get; set; }=DateTime.Now;
        public AppUser? AppUser { get; set; }
        public Product? Product { get; set; }
    }
}