using System.ComponentModel.DataAnnotations;

namespace MomoMats.Models;

public class OrderItem
{
    public int Id { get; set; }


    public int OrderId { get; set; }


    public int MatId { get; set; }


    [Required]
    [MaxLength(100)]
    public string MatName { get; set; } = string.Empty;


    [Required]
    [MaxLength(20)]
    public string Provider { get; set; } = string.Empty;


    [Range(1, 10)]
    public int Quantity { get; set; }


    public decimal UnitPrice { get; set; }


    // Navigation to the parent order.
    public Order Order { get; set; } = null!;


    // Navigation to the original mat.
    public Mat Mat { get; set; } = null!;
}