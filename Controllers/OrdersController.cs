using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MomoMats.Data;
using MomoMats.Models;

namespace MomoMats.Controllers;


[ApiController]
[Authorize]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly MomoMatsDbContext _dbContext;

    private readonly UserManager<ApplicationUser> _userManager;


    public OrdersController(
        MomoMatsDbContext dbContext,
        UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;

        _userManager = userManager;
    }


    // ---------------------------------------------------------
    // GET CURRENT USER'S ORDERS
    //
    // GET: /api/orders
    // ---------------------------------------------------------

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderResponse>>>
        GetOrders()
    {
        string? userId =
            _userManager.GetUserId(User);


        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }


        List<Order> orders =
            await _dbContext.Orders
                .AsNoTracking()
                .Include(order =>
                    order.OrderItems)
                .Where(order =>
                    order.UserId == userId)
                .OrderByDescending(order =>
                    order.CreatedAt)
                .ToListAsync();


        List<OrderResponse> response =
            orders
                .Select(MapOrderResponse)
                .ToList();


        return Ok(response);
    }


    // ---------------------------------------------------------
    // GET ONE ORDER
    //
    // GET: /api/orders/1
    // ---------------------------------------------------------

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderResponse>>
        GetOrderById(int id)
    {
        string? userId =
            _userManager.GetUserId(User);


        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }


        Order? order =
            await _dbContext.Orders
                .AsNoTracking()
                .Include(existingOrder =>
                    existingOrder.OrderItems)
                .FirstOrDefaultAsync(existingOrder =>
                    existingOrder.Id == id &&
                    existingOrder.UserId == userId);


        if (order is null)
        {
            return NotFound(new
            {
                message =
                    $"Order with ID {id} was not found."
            });
        }


        return Ok(
            MapOrderResponse(order)
        );
    }


    // ---------------------------------------------------------
    // CHECKOUT / CREATE ORDER
    //
    // POST: /api/orders
    // ---------------------------------------------------------

    [HttpPost]
    public async Task<ActionResult<OrderResponse>>
        CreateOrder()
    {
        string? userId =
            _userManager.GetUserId(User);


        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }


        List<CartItem> cartItems =
            await _dbContext.CartItems
                .Include(cartItem =>
                    cartItem.Mat)
                .Where(cartItem =>
                    cartItem.UserId == userId)
                .ToListAsync();


        if (cartItems.Count == 0)
        {
            return BadRequest(new
            {
                message =
                    "Your cart is empty."
            });
        }


        decimal totalAmount =
            cartItems.Sum(cartItem =>
                cartItem.Mat.Price *
                cartItem.Quantity);


        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync();


        try
        {
            var order = new Order
            {
                UserId = userId,

                CreatedAt =
                    DateTimeOffset.UtcNow,

                Status = "Placed",

                TotalAmount = totalAmount,

                OrderItems =
                    cartItems
                        .Select(cartItem =>
                            new OrderItem
                            {
                                MatId =
                                    cartItem.MatId,

                                MatName =
                                    cartItem.Mat.Name,

                                Provider =
                                    cartItem.Mat.Provider,

                                Quantity =
                                    cartItem.Quantity,

                                UnitPrice =
                                    cartItem.Mat.Price
                            })
                        .ToList()
            };


            _dbContext.Orders.Add(order);


            _dbContext.CartItems
                .RemoveRange(cartItems);


            await _dbContext
                .SaveChangesAsync();


            await transaction
                .CommitAsync();


            return CreatedAtAction(
                nameof(GetOrderById),

                new
                {
                    id = order.Id
                },

                MapOrderResponse(order)
            );
        }
        catch
        {
            await transaction
                .RollbackAsync();

            throw;
        }
    }


    // ---------------------------------------------------------
    // MAP DATABASE ORDER TO API RESPONSE
    // ---------------------------------------------------------

    private static OrderResponse MapOrderResponse(
        Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,

            CreatedAt =
                order.CreatedAt,

            Status =
                order.Status,

            Total =
                order.TotalAmount,

            Items =
                order.OrderItems
                    .Select(orderItem =>
                        new OrderItemResponse
                        {
                            MatId =
                                orderItem.MatId,

                            MatName =
                                orderItem.MatName,

                            Provider =
                                orderItem.Provider,

                            Quantity =
                                orderItem.Quantity,

                            UnitPrice =
                                orderItem.UnitPrice,

                            LineTotal =
                                orderItem.UnitPrice *
                                orderItem.Quantity
                        })
                    .ToList()
        };
    }
}


// ---------------------------------------------------------
// RESPONSE TYPES
// ---------------------------------------------------------

public sealed class OrderResponse
{
    public int Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public string Status { get; set; }
        = string.Empty;

    public decimal Total { get; set; }

    public List<OrderItemResponse> Items { get; set; }
        = new();
}


public sealed class OrderItemResponse
{
    public int MatId { get; set; }

    public string MatName { get; set; }
        = string.Empty;

    public string Provider { get; set; }
        = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal LineTotal { get; set; }
}