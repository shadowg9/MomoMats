using Microsoft.AspNetCore.Mvc;

namespace MomoMats.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    // GET: /api/orders/demo-user
    [HttpGet("{userId}")]
    public ActionResult<IEnumerable<OrderDto>> GetOrders(
        string userId)
    {
        lock (DemoStore.SyncRoot)
        {
            var orders = DemoStore.Orders
                .Where(order =>
                    order.UserId.Equals(
                        userId,
                        StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(
                    order => order.CreatedAt)
                .ToList();


            return Ok(orders);
        }
    }


    // GET: /api/orders/demo-user/1
    [HttpGet("{userId}/{id:int}")]
    public ActionResult<OrderDto> GetOrder(
        string userId,
        int id)
    {
        lock (DemoStore.SyncRoot)
        {
            OrderDto? order = DemoStore.Orders
                .FirstOrDefault(order =>
                    order.Id == id &&
                    order.UserId.Equals(
                        userId,
                        StringComparison.OrdinalIgnoreCase));


            if (order is null)
            {
                return NotFound(new
                {
                    message = $"Order {id} was not found."
                });
            }


            return Ok(order);
        }
    }


    // POST: /api/orders/demo-user
    [HttpPost("{userId}")]
    public ActionResult<OrderDto> Checkout(string userId)
    {
        lock (DemoStore.SyncRoot)
        {
            if (!DemoStore.Carts.TryGetValue(userId, out var cart) ||
                cart.Count == 0)
            {
                return BadRequest(new
                {
                    message = "Your cart is empty."
                });
            }


            var orderItems = new List<OrderItemDto>();


            foreach (CartLine cartLine in cart)
            {
                MatDto? mat = DemoStore.Mats
                    .FirstOrDefault(
                        mat => mat.Id == cartLine.MatId);


                if (mat is null)
                {
                    continue;
                }


                orderItems.Add(new OrderItemDto
                {
                    MatId = mat.Id,
                    MatName = mat.Name,
                    Provider = mat.Provider,
                    Quantity = cartLine.Quantity,
                    UnitPrice = mat.Price
                });
            }


            var order = new OrderDto
            {
                Id = DemoStore.NextOrderId(),

                UserId = userId,

                CreatedAt = DateTimeOffset.UtcNow,

                Status = "Order Placed",

                Items = orderItems,

                Total = orderItems.Sum(
                    item =>
                        item.UnitPrice *
                        item.Quantity)
            };


            DemoStore.Orders.Add(order);

            DemoStore.Carts[userId] = [];


            return CreatedAtAction(
                nameof(GetOrder),
                new
                {
                    userId,
                    id = order.Id
                },
                order);
        }
    }
}


public sealed class OrderDto
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }

    public string Status { get; set; } = string.Empty;

    public List<OrderItemDto> Items { get; set; } = [];

    public decimal Total { get; set; }
}


public sealed class OrderItemDto
{
    public int MatId { get; set; }

    public string MatName { get; set; } = string.Empty;

    public string Provider { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal LineTotal =>
        UnitPrice * Quantity;
}