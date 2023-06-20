using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ETIT.Models.Auth;

public class AppUser:IdentityUser
{
    public AppUser()
    {
        Orders = new List<Order>();
        Deliveries = new List<Delivery>();
    }
    [Required,MaxLength(100)]
    public string FullName { get; set; }        
    [Required, MaxLength(255)]
    public string Address { get; set; }
    public virtual List<Order> Orders { get; set; }
    public virtual List<Delivery> Deliveries { get; set; }

}
