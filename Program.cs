using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MomoMats.Data;
using MomoMats.Models;


var builder = WebApplication.CreateBuilder(args);


// ---------------------------------------------------------
// CONTROLLERS
// ---------------------------------------------------------

builder.Services.AddControllers();


// ---------------------------------------------------------
// OPENAPI / SWAGGER
// ---------------------------------------------------------

builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();


// ---------------------------------------------------------
// DATABASE / ENTITY FRAMEWORK CORE / MYSQL
// ---------------------------------------------------------

builder.Services.AddDbContext<MomoMatsDbContext>(options =>
{
    string connectionString =
        builder.Configuration
            .GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
            "Connection string 'DefaultConnection' was not found.");

    options.UseMySQL(connectionString);
});


// ---------------------------------------------------------
// ASP.NET CORE IDENTITY
// ---------------------------------------------------------

builder.Services.AddAuthorization();

builder.Services
    .AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<MomoMatsDbContext>();




// ---------------------------------------------------------
// BUILD APPLICATION
// ---------------------------------------------------------

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext =
        scope.ServiceProvider.GetRequiredService<MomoMatsDbContext>();

    await dbContext.Database.MigrateAsync();
    await DbInitializer.SeedAsync(dbContext);
}

// ---------------------------------------------------------
// SEED INITIAL DATABASE DATA
// ---------------------------------------------------------

//using (IServiceScope scope = app.Services.CreateScope())
//{
//    MomoMatsDbContext dbContext =
//        scope.ServiceProvider
//            .GetRequiredService<MomoMatsDbContext>();

//    await DbInitializer.SeedAsync(dbContext);
//}


// ---------------------------------------------------------
// DEVELOPMENT TOOLS
// ---------------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();

    app.UseSwaggerUI();
}


// ---------------------------------------------------------
// HTTP REQUEST PIPELINE
// ---------------------------------------------------------

app.UseHttpsRedirection();


// Serve index.html, app.js, and styles.css.
app.UseStaticFiles();


// Authentication must run before authorization.
app.UseAuthentication();

app.UseAuthorization();


// ---------------------------------------------------------
// HOMEPAGE
// ---------------------------------------------------------

app.MapGet(
    "/",
    () => Results.Redirect("/index.html")
);


// ---------------------------------------------------------
// API CONTROLLERS
// ---------------------------------------------------------

app.MapControllers();


// ---------------------------------------------------------
// IDENTITY API ENDPOINTS
//
// Examples:
// POST /api/auth/register
// POST /api/auth/login
// POST /api/auth/refresh
// ---------------------------------------------------------

app.MapGroup("/api/auth")
    .MapIdentityApi<ApplicationUser>();


// ---------------------------------------------------------
// LOGOUT ENDPOINT
// POST: /api/auth/logout
// ---------------------------------------------------------

app.MapPost(
    "/api/auth/logout",
    async (
        SignInManager<ApplicationUser> signInManager) =>
    {
        await signInManager.SignOutAsync();

        return Results.Ok(new
        {
            message = "Logged out successfully."
        });
    })
    .RequireAuthorization();


// ---------------------------------------------------------
// CURRENT USER ENDPOINT
// GET: /api/auth/me
// ---------------------------------------------------------

app.MapGet(
    "/api/auth/me",
    async (
        HttpContext httpContext,
        UserManager<ApplicationUser> userManager) =>
    {
        ApplicationUser? user =
            await userManager.GetUserAsync(
                httpContext.User);

        if (user is null)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(new
        {
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName
        });
    })
    .RequireAuthorization();


// ---------------------------------------------------------
// START APPLICATION
// ---------------------------------------------------------

app.Run();