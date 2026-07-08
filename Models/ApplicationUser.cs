using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MomoMats.Models;

public class ApplicationUser : IdentityUser
{
    [MaxLength(50)]
    public string? FirstName { get; set; }


    [MaxLength(50)]
    public string? LastName { get; set; }


    public DateTimeOffset CreatedAt { get; set; }
        = DateTimeOffset.UtcNow;


    // Navigation property:
    // One user can have many cart items.
    public ICollection<CartItem> CartItems { get; set; }
        = new List<CartItem>();


    // Navigation property:
    // One user can have many orders.
    public ICollection<Order> Orders { get; set; }
        = new List<Order>();
}