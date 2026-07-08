using System.ComponentModel.DataAnnotations;

namespace MomoMats.Models;

public class Mat
{
    public int Id { get; set; }


    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;


    [Required]
    [MaxLength(20)]
    public string Provider { get; set; } = string.Empty;


    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;


    [MaxLength(2000)]
    public string? GenerationPrompt { get; set; }


    [MaxLength(2048)]
    public string? ImageUrl { get; set; }


    public decimal Price { get; set; }


    public DateTimeOffset CreatedAt { get; set; }
        = DateTimeOffset.UtcNow;


    // A mat can appear in many users' carts.
    public ICollection<CartItem> CartItems { get; set; }
        = new List<CartItem>();


    // A mat can appear in many order items.
    public ICollection<OrderItem> OrderItems { get; set; }
        = new List<OrderItem>();
}