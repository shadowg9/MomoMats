using Microsoft.AspNetCore.Mvc;

namespace MomoMats.Controllers;

[ApiController]
[Route("api/mats")]
public class MatsController : ControllerBase
{
    // GET: /api/mats
    [HttpGet]
    public ActionResult<IEnumerable<MatDto>> GetAllMats()
    {
        lock (DemoStore.SyncRoot)
        {
            return Ok(DemoStore.Mats.ToList());
        }
    }


    // GET: /api/mats/openai
    [HttpGet("openai")]
    public ActionResult<IEnumerable<MatDto>> GetOpenAiMats()
    {
        lock (DemoStore.SyncRoot)
        {
            var mats = DemoStore.Mats
                .Where(mat =>
                    mat.Provider.Equals(
                        "OpenAI",
                        StringComparison.OrdinalIgnoreCase))
                .Take(5)
                .ToList();

            return Ok(mats);
        }
    }


    // GET: /api/mats/gemini
    [HttpGet("gemini")]
    public ActionResult<IEnumerable<MatDto>> GetGeminiMats()
    {
        lock (DemoStore.SyncRoot)
        {
            var mats = DemoStore.Mats
                .Where(mat =>
                    mat.Provider.Equals(
                        "Gemini",
                        StringComparison.OrdinalIgnoreCase))
                .Take(5)
                .ToList();

            return Ok(mats);
        }
    }


    // GET: /api/mats/1
    [HttpGet("{id:int}")]
    public ActionResult<MatDto> GetMatById(int id)
    {
        lock (DemoStore.SyncRoot)
        {
            MatDto? mat = DemoStore.Mats
                .FirstOrDefault(mat => mat.Id == id);

            if (mat is null)
            {
                return NotFound(new
                {
                    message = $"Mat with ID {id} was not found."
                });
            }

            return Ok(mat);
        }
    }
}


public sealed class MatDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Provider { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}


/*
 * Temporary in-memory store.
 *
 * Next stage:
 * - Mats will come from OpenAI and Gemini provider services.
 * - Generated mat records will be persisted in MySQL/RDS.
 * - Carts and orders will also move to the database.
 */
internal static class DemoStore
{
    public static readonly object SyncRoot = new();

    private static int _nextOrderId = 0;


    public static int NextOrderId()
    {
        return Interlocked.Increment(ref _nextOrderId);
    }


    public static List<MatDto> Mats { get; } =
    [
        // ---------------------------------------------------------
        // OPENAI DESIGNS
        // ---------------------------------------------------------

        new MatDto
        {
            Id = 1,
            Name = "Cosmic Welcome",
            Description =
                "A deep-space welcome mat with colorful nebula clouds and stars.",
            Provider = "OpenAI",
            Price = 29.99m,
            ImageUrl = null,
            CreatedAt = DateTimeOffset.UtcNow
        },

        new MatDto
        {
            Id = 2,
            Name = "Neon Circuit",
            Description =
                "A futuristic cyberpunk mat inspired by glowing circuit boards.",
            Provider = "OpenAI",
            Price = 34.99m,
            ImageUrl = null,
            CreatedAt = DateTimeOffset.UtcNow
        },

        new MatDto
        {
            Id = 3,
            Name = "Mountain Sunrise",
            Description =
                "A peaceful mountain landscape illuminated by an abstract sunrise.",
            Provider = "OpenAI",
            Price = 31.99m,
            ImageUrl = null,
            CreatedAt = DateTimeOffset.UtcNow
        },

        new MatDto
        {
            Id = 4,
            Name = "Retro Arcade",
            Description =
                "A colorful pixel-art design inspired by classic arcade games.",
            Provider = "OpenAI",
            Price = 32.99m,
            ImageUrl = null,
            CreatedAt = DateTimeOffset.UtcNow
        },

        new MatDto
        {
            Id = 5,
            Name = "Midnight Garden",
            Description =
                "A dark botanical pattern with glowing flowers and moonlit leaves.",
            Provider = "OpenAI",
            Price = 35.99m,
            ImageUrl = null,
            CreatedAt = DateTimeOffset.UtcNow
        },


        // ---------------------------------------------------------
        // GEMINI DESIGNS
        // ---------------------------------------------------------

        new MatDto
        {
            Id = 6,
            Name = "Botanical Geometry",
            Description =
                "A modern combination of geometric shapes and botanical elements.",
            Provider = "Gemini",
            Price = 30.99m,
            ImageUrl = null,
            CreatedAt = DateTimeOffset.UtcNow
        },

        new MatDto
        {
            Id = 7,
            Name = "Ocean Mosaic",
            Description =
                "An abstract ocean design featuring layered blue mosaic patterns.",
            Provider = "Gemini",
            Price = 33.99m,
            ImageUrl = null,
            CreatedAt = DateTimeOffset.UtcNow
        },

        new MatDto
        {
            Id = 8,
            Name = "Desert Horizon",
            Description =
                "A warm minimalist desert scene with dunes and a setting sun.",
            Provider = "Gemini",
            Price = 29.99m,
            ImageUrl = null,
            CreatedAt = DateTimeOffset.UtcNow
        },

        new MatDto
        {
            Id = 9,
            Name = "Forest Path",
            Description =
                "A quiet illustrated forest trail surrounded by deep green foliage.",
            Provider = "Gemini",
            Price = 34.99m,
            ImageUrl = null,
            CreatedAt = DateTimeOffset.UtcNow
        },

        new MatDto
        {
            Id = 10,
            Name = "Abstract Waves",
            Description =
                "A contemporary flowing pattern inspired by colorful ocean currents.",
            Provider = "Gemini",
            Price = 31.99m,
            ImageUrl = null,
            CreatedAt = DateTimeOffset.UtcNow
        }
    ];


    public static Dictionary<string, List<CartLine>> Carts { get; } =
        new(StringComparer.OrdinalIgnoreCase);


    public static List<OrderDto> Orders { get; } = [];
}