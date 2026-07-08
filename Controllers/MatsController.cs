using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MomoMats.Data;

namespace MomoMats.Controllers;

[ApiController]
[Route("api/mats")]
public class MatsController : ControllerBase
{
    private readonly MomoMatsDbContext _dbContext;


    public MatsController(MomoMatsDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    // ---------------------------------------------------------
    // GET ALL MATS
    // GET: /api/mats
    // ---------------------------------------------------------

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MatDto>>> GetAllMats()
    {
        var mats = await _dbContext.Mats
            .AsNoTracking()
            .OrderBy(mat => mat.Id)
            .Select(mat => new MatDto
            {
                Id = mat.Id,
                Name = mat.Name,
                Description = mat.Description,
                Provider = mat.Provider,
                Price = mat.Price,
                ImageUrl = mat.ImageUrl,
                CreatedAt = mat.CreatedAt
            })
            .ToListAsync();

        return Ok(mats);
    }


    // ---------------------------------------------------------
    // GET OPENAI MATS
    // GET: /api/mats/openai
    // ---------------------------------------------------------

    [HttpGet("openai")]
    public async Task<ActionResult<IEnumerable<MatDto>>> GetOpenAiMats()
    {
        var mats = await _dbContext.Mats
            .AsNoTracking()
            .Where(mat => mat.Provider == "OpenAI")
            .OrderBy(mat => mat.Id)
            .Take(5)
            .Select(mat => new MatDto
            {
                Id = mat.Id,
                Name = mat.Name,
                Description = mat.Description,
                Provider = mat.Provider,
                Price = mat.Price,
                ImageUrl = mat.ImageUrl,
                CreatedAt = mat.CreatedAt
            })
            .ToListAsync();

        return Ok(mats);
    }


    // ---------------------------------------------------------
    // GET GEMINI MATS
    // GET: /api/mats/gemini
    // ---------------------------------------------------------

    [HttpGet("gemini")]
    public async Task<ActionResult<IEnumerable<MatDto>>> GetGeminiMats()
    {
        var mats = await _dbContext.Mats
            .AsNoTracking()
            .Where(mat => mat.Provider == "Gemini")
            .OrderBy(mat => mat.Id)
            .Take(5)
            .Select(mat => new MatDto
            {
                Id = mat.Id,
                Name = mat.Name,
                Description = mat.Description,
                Provider = mat.Provider,
                Price = mat.Price,
                ImageUrl = mat.ImageUrl,
                CreatedAt = mat.CreatedAt
            })
            .ToListAsync();

        return Ok(mats);
    }


    // ---------------------------------------------------------
    // GET MAT BY ID
    // GET: /api/mats/1
    // ---------------------------------------------------------

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MatDto>> GetMatById(int id)
    {
        MatDto? mat = await _dbContext.Mats
            .AsNoTracking()
            .Where(mat => mat.Id == id)
            .Select(mat => new MatDto
            {
                Id = mat.Id,
                Name = mat.Name,
                Description = mat.Description,
                Provider = mat.Provider,
                Price = mat.Price,
                ImageUrl = mat.ImageUrl,
                CreatedAt = mat.CreatedAt
            })
            .FirstOrDefaultAsync();


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


// ---------------------------------------------------------
// MAT API RESPONSE DTO
// ---------------------------------------------------------

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


