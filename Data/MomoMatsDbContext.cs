using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MomoMats.Models;
using System.Reflection.Emit;

namespace MomoMats.Data;

public class MomoMatsDbContext
    : IdentityDbContext<ApplicationUser>
{
    public MomoMatsDbContext(
        DbContextOptions<MomoMatsDbContext> options)
        : base(options)
    {
    }


    // ---------------------------------------------------------
    // MOMOMATS TABLES
    // ---------------------------------------------------------

    public DbSet<Mat> Mats => Set<Mat>();

    public DbSet<CartItem> CartItems => Set<CartItem>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();


    // ---------------------------------------------------------
    // DATABASE MODEL CONFIGURATION
    // ---------------------------------------------------------

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        // Required so ASP.NET Core Identity can configure
        // its own users, roles, claims, tokens, etc.
        base.OnModelCreating(modelBuilder);


        ConfigureMat(modelBuilder);

        ConfigureCartItem(modelBuilder);

        ConfigureOrder(modelBuilder);

        ConfigureOrderItem(modelBuilder);
    }


    // ---------------------------------------------------------
    // MAT CONFIGURATION
    // ---------------------------------------------------------

    private static void ConfigureMat(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Mat>(entity =>
        {
            entity.ToTable("Mats");


            entity.HasKey(mat => mat.Id);


            entity.Property(mat => mat.Name)
                .HasMaxLength(100)
                .IsRequired();


            entity.Property(mat => mat.Provider)
                .HasMaxLength(20)
                .IsRequired();


            entity.Property(mat => mat.Description)
                .HasMaxLength(1000)
                .IsRequired();


            entity.Property(mat => mat.GenerationPrompt)
                .HasMaxLength(2000);


            entity.Property(mat => mat.ImageUrl)
                .HasMaxLength(2048);


            entity.Property(mat => mat.Price)
                .HasPrecision(10, 2);


            entity.HasIndex(mat => mat.Provider);


            entity.HasIndex(mat => mat.CreatedAt);
        });
    }


    // ---------------------------------------------------------
    // CART ITEM CONFIGURATION
    // ---------------------------------------------------------

    private static void ConfigureCartItem(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.ToTable("CartItems");


            entity.HasKey(cartItem => cartItem.Id);


            entity.Property(cartItem => cartItem.UserId)
                .IsRequired();


            entity.Property(cartItem => cartItem.Quantity)
                .IsRequired();


            /*
             * Prevents duplicate rows such as:
             *
             * User A + Mat 3
             * User A + Mat 3
             *
             * Instead, the application should update Quantity.
             */
            entity.HasIndex(
                    cartItem => new
                    {
                        cartItem.UserId,
                        cartItem.MatId
                    })
                .IsUnique();


            /*
             * ApplicationUser
             *      1
             *      |
             *      *
             * CartItem
             */
            entity.HasOne(cartItem => cartItem.User)
                .WithMany(user => user.CartItems)
                .HasForeignKey(cartItem => cartItem.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            /*
             * Mat
             *  1
             *  |
             *  *
             * CartItem
             */
            entity.HasOne(cartItem => cartItem.Mat)
                .WithMany(mat => mat.CartItems)
                .HasForeignKey(cartItem => cartItem.MatId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }


    // ---------------------------------------------------------
    // ORDER CONFIGURATION
    // ---------------------------------------------------------

    private static void ConfigureOrder(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");


            entity.HasKey(order => order.Id);


            entity.Property(order => order.UserId)
                .IsRequired();


            entity.Property(order => order.Status)
                .HasMaxLength(30)
                .IsRequired();


            entity.Property(order => order.TotalAmount)
                .HasPrecision(10, 2);


            entity.HasIndex(order => order.UserId);


            entity.HasIndex(order => order.CreatedAt);


            /*
             * ApplicationUser
             *      1
             *      |
             *      *
             * Order
             */
            entity.HasOne(order => order.User)
                .WithMany(user => user.Orders)
                .HasForeignKey(order => order.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }


    // ---------------------------------------------------------
    // ORDER ITEM CONFIGURATION
    // ---------------------------------------------------------

    private static void ConfigureOrderItem(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItems");


            entity.HasKey(orderItem => orderItem.Id);


            entity.Property(orderItem => orderItem.MatName)
                .HasMaxLength(100)
                .IsRequired();


            entity.Property(orderItem => orderItem.Provider)
                .HasMaxLength(20)
                .IsRequired();


            entity.Property(orderItem => orderItem.Quantity)
                .IsRequired();


            entity.Property(orderItem => orderItem.UnitPrice)
                .HasPrecision(10, 2);


            /*
             * Order
             *   1
             *   |
             *   *
             * OrderItem
             */
            entity.HasOne(orderItem => orderItem.Order)
                .WithMany(order => order.OrderItems)
                .HasForeignKey(orderItem => orderItem.OrderId)
                .OnDelete(DeleteBehavior.Cascade);


            /*
             * Mat
             *  1
             *  |
             *  *
             * OrderItem
             *
             * Restrict is intentional:
             * don't delete a product referenced by order history.
             */
            entity.HasOne(orderItem => orderItem.Mat)
                .WithMany(mat => mat.OrderItems)
                .HasForeignKey(orderItem => orderItem.MatId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}