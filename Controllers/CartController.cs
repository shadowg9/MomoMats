using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MomoMats.Data;
using MomoMats.Models;

namespace MomoMats.Controllers;


[ApiController]
[Authorize]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly MomoMatsDbContext _dbContext;

    private readonly UserManager<ApplicationUser> _userManager;


    public CartController(
        MomoMatsDbContext dbContext,
        UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;

        _userManager = userManager;
    }


    // ---------------------------------------------------------
    // GET CURRENT USER'S CART
    //
    // GET: /api/cart
    // ---------------------------------------------------------

    [HttpGet]
    public async Task<ActionResult<CartResponse>> GetCart()
    {
        string? userId =
            _userManager.GetUserId(User);


        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }


        CartResponse cart =
            await BuildCartResponseAsync(userId);


        return Ok(cart);
    }


    // ---------------------------------------------------------
    // ADD ITEM TO CART
    //
    // POST: /api/cart/items
    // ---------------------------------------------------------

    [HttpPost("items")]
    public async Task<ActionResult<CartResponse>> AddItem(
        AddCartItemRequest request)
    {
        string? userId =
            _userManager.GetUserId(User);


        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }


        if (request.Quantity < 1 ||
            request.Quantity > 10)
        {
            return BadRequest(new
            {
                message =
                    "Quantity must be between 1 and 10."
            });
        }


        bool matExists =
            await _dbContext.Mats
                .AnyAsync(mat =>
                    mat.Id == request.MatId);


        if (!matExists)
        {
            return NotFound(new
            {
                message =
                    $"Mat with ID {request.MatId} was not found."
            });
        }


        CartItem? existingItem =
            await _dbContext.CartItems
                .FirstOrDefaultAsync(cartItem =>
                    cartItem.UserId == userId &&
                    cartItem.MatId == request.MatId);


        if (existingItem is null)
        {
            var cartItem = new CartItem
            {
                UserId = userId,

                MatId = request.MatId,

                Quantity = request.Quantity,

                AddedAt = DateTimeOffset.UtcNow
            };


            _dbContext.CartItems.Add(cartItem);
        }
        else
        {
            int newQuantity =
                existingItem.Quantity +
                request.Quantity;


            if (newQuantity > 10)
            {
                return BadRequest(new
                {
                    message =
                        "A cart item cannot have a quantity greater than 10."
                });
            }


            existingItem.Quantity =
                newQuantity;
        }


        await _dbContext.SaveChangesAsync();


        CartResponse cart =
            await BuildCartResponseAsync(userId);


        return Ok(cart);
    }


    // ---------------------------------------------------------
    // REMOVE ITEM FROM CART
    //
    // DELETE: /api/cart/items/1
    // ---------------------------------------------------------

    [HttpDelete("items/{matId:int}")]
    public async Task<ActionResult<CartResponse>> RemoveItem(
        int matId)
    {
        string? userId =
            _userManager.GetUserId(User);


        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }


        CartItem? cartItem =
            await _dbContext.CartItems
                .FirstOrDefaultAsync(item =>
                    item.UserId == userId &&
                    item.MatId == matId);


        if (cartItem is null)
        {
            return NotFound(new
            {
                message =
                    "The requested mat was not found in your cart."
            });
        }


        _dbContext.CartItems.Remove(cartItem);


        await _dbContext.SaveChangesAsync();


        CartResponse cart =
            await BuildCartResponseAsync(userId);


        return Ok(cart);
    }


    // ---------------------------------------------------------
    // BUILD CART RESPONSE
    // ---------------------------------------------------------

    private async Task<CartResponse> BuildCartResponseAsync(
        string userId)
    {
        List<CartItemResponse> items =
            await _dbContext.CartItems
                .AsNoTracking()
                .Where(cartItem =>
                    cartItem.UserId == userId)
                .OrderBy(cartItem =>
                    cartItem.AddedAt)
                .Select(cartItem =>
                    new CartItemResponse
                    {
                        MatId =
                            cartItem.MatId,

                        Name =
                            cartItem.Mat.Name,

                        Provider =
                            cartItem.Mat.Provider,

                        Quantity =
                            cartItem.Quantity,

                        UnitPrice =
                            cartItem.Mat.Price,

                        LineTotal =
                            cartItem.Mat.Price *
                            cartItem.Quantity,

                        ImageUrl =
                            cartItem.Mat.ImageUrl
                    })
                .ToListAsync();


        return new CartResponse
        {
            Items = items,

            Total = items.Sum(item =>
                item.LineTotal)
        };
    }
}


// ---------------------------------------------------------
// REQUEST / RESPONSE TYPES
// ---------------------------------------------------------

public sealed class AddCartItemRequest
{
    public int MatId { get; set; }

    public int Quantity { get; set; } = 1;
}


public sealed class CartResponse
{
    public List<CartItemResponse> Items { get; set; }
        = new();

    public decimal Total { get; set; }
}


public sealed class CartItemResponse
{
    public int MatId { get; set; }

    public string Name { get; set; }
        = string.Empty;

    public string Provider { get; set; }
        = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal LineTotal { get; set; }

    public string? ImageUrl { get; set; }
}