using System.ComponentModel.DataAnnotations;

namespace MomoMats.Models;

public class CartItem
{
    public int Id { get; set; }


    [Required]
    public string UserId { get; set; } = string.Empty;


    public int MatId { get; set; }


    [Range(1, 10)]
    public int Quantity { get; set; } = 1;


    public DateTimeOffset AddedAt { get; set; }
        = DateTimeOffset.UtcNow;


    // Navigation to the user who owns the cart item.
    public ApplicationUser User { get; set; } = null!;


    // Navigation to the mat being purchased.
    public Mat Mat { get; set; } = null!;
}