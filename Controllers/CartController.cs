using Microsoft.AspNetCore.Mvc;

namespace MomoMats.Controllers;

[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    // GET: /api/cart/demo-user
    [HttpGet("{userId}")]
    public ActionResult<CartResponse> GetCart(string userId)
    {
        lock (DemoStore.SyncRoot)
        {
            return Ok(BuildCartResponse(userId));
        }
    }


    // POST: /api/cart/demo-user/items
    [HttpPost("{userId}/items")]
    public ActionResult<CartResponse> AddItem(
        string userId,
        AddCartItemRequest request)
    {
        if (request.Quantity < 1 || request.Quantity > 10)
        {
            return BadRequest(new
            {
                message = "Quantity must be between 1 and 10."
            });
        }


        lock (DemoStore.SyncRoot)
        {
            MatDto? mat = DemoStore.Mats
                .FirstOrDefault(mat => mat.Id == request.MatId);


            if (mat is null)
            {
                return NotFound(new
                {
                    message =
                        $"Mat with ID {request.MatId} was not found."
                });
            }


            if (!DemoStore.Carts.TryGetValue(userId, out var cart))
            {
                cart = [];
                DemoStore.Carts[userId] = cart;
            }


            CartLine? existingItem = cart
                .FirstOrDefault(
                    item => item.MatId == request.MatId);


            if (existingItem is null)
            {
                cart.Add(new CartLine
                {
                    MatId = request.MatId,
                    Quantity = request.Quantity
                });
            }
            else
            {
                existingItem.Quantity = Math.Min(
                    existingItem.Quantity + request.Quantity,
                    10);
            }


            return Ok(BuildCartResponse(userId));
        }
    }


    // DELETE: /api/cart/demo-user/items/1
    [HttpDelete("{userId}/items/{matId:int}")]
    public ActionResult<CartResponse> RemoveItem(
        string userId,
        int matId)
    {
        lock (DemoStore.SyncRoot)
        {
            if (DemoStore.Carts.TryGetValue(userId, out var cart))
            {
                CartLine? item = cart
                    .FirstOrDefault(
                        item => item.MatId == matId);


                if (item is not null)
                {
                    cart.Remove(item);
                }
            }


            return Ok(BuildCartResponse(userId));
        }
    }


    internal static CartResponse BuildCartResponse(string userId)
    {
        if (!DemoStore.Carts.TryGetValue(userId, out var cart))
        {
            cart = [];
        }


        var items = cart
            .Select(cartLine =>
            {
                MatDto? mat = DemoStore.Mats
                    .FirstOrDefault(
                        mat => mat.Id == cartLine.MatId);


                if (mat is null)
                {
                    return null;
                }


                return new CartItemResponse
                {
                    MatId = mat.Id,
                    Name = mat.Name,
                    Provider = mat.Provider,
                    Price = mat.Price,
                    Quantity = cartLine.Quantity
                };
            })
            .Where(item => item is not null)
            .Cast<CartItemResponse>()
            .ToList();


        return new CartResponse
        {
            UserId = userId,
            Items = items,

            Total = items.Sum(
                item =>
                    item.Price *
                    item.Quantity)
        };
    }
}


public sealed class AddCartItemRequest
{
    public int MatId { get; set; }

    public int Quantity { get; set; } = 1;
}


public sealed class CartLine
{
    public int MatId { get; set; }

    public int Quantity { get; set; }
}


public sealed class CartItemResponse
{
    public int MatId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Provider { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public decimal LineTotal =>
        Price * Quantity;
}


public sealed class CartResponse
{
    public string UserId { get; set; } = string.Empty;

    public List<CartItemResponse> Items { get; set; } = [];

    public decimal Total { get; set; }
}