using System.ComponentModel.DataAnnotations;

namespace MomoMats.Models;

public class Order
{
    public int Id { get; set; }


    [Required]
    public string UserId { get; set; } = string.Empty;


    public DateTimeOffset CreatedAt { get; set; }
        = DateTimeOffset.UtcNow;


    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = "Pending";


    public decimal TotalAmount { get; set; }


    // User who placed the order.
    public ApplicationUser User { get; set; } = null!;


    // Products included in this order.
    public ICollection<OrderItem> OrderItems { get; set; }
        = new List<OrderItem>();
}