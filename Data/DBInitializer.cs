using Microsoft.EntityFrameworkCore;
using MomoMats.Models;
using static System.Net.WebRequestMethods;

namespace MomoMats.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(
        MomoMatsDbContext dbContext)
    {
        // If mats already exist, don't insert duplicates.
        if (await dbContext.Mats.AnyAsync())
        {
            return;
        }

        var mats = new List<Mat>
        {
            // =====================================================
            // OPENAI MATS
            // =====================================================

            new Mat
            {
                Name = "Cosmic Welcome",

                Description =
                    "A deep-space welcome mat with colorful nebula clouds and stars.",

                Provider = "OpenAI",

                GenerationPrompt =
                    "Create a rectangular decorative doormat inspired by deep space, colorful nebula clouds, glowing stars, and a premium woven texture.",

                ImageUrl = https://momomats.s3.us-east-1.amazonaws.com/ChatGPT_Cosmic_Welcome.png,

                Price = 29.99m,

                CreatedAt = DateTimeOffset.UtcNow
            },

            new Mat
            {
                Name = "Neon Circuit",

                Description =
                    "A futuristic cyberpunk mat inspired by glowing circuit boards.",

                Provider = "OpenAI",

                GenerationPrompt =
                    "Create a futuristic cyberpunk doormat featuring glowing neon circuit-board patterns, dark tones, and bright technological accents.",

                ImageUrl = https://momomats.s3.us-east-1.amazonaws.com/ChatGPT_Neon_Circuit.png,

                Price = 34.99m,

                CreatedAt = DateTimeOffset.UtcNow
            },

            new Mat
            {
                Name = "Mountain Sunrise",

                Description =
                    "A peaceful mountain landscape illuminated by an abstract sunrise.",

                Provider = "OpenAI",

                GenerationPrompt =
                    "Create a decorative doormat featuring layered mountain silhouettes and an abstract colorful sunrise with a calm minimalist aesthetic.",

                ImageUrl = https://momomats.s3.us-east-1.amazonaws.com/ChatGPT_Mountain_Sunrise.png,

                Price = 31.99m,

                CreatedAt = DateTimeOffset.UtcNow
            },

            new Mat
            {
                Name = "Retro Arcade",

                Description =
                    "A colorful pixel-art design inspired by classic arcade games.",

                Provider = "OpenAI",

                GenerationPrompt =
                    "Create a colorful pixel-art doormat inspired by classic 1980s arcade games, neon signs, pixels, and retro gaming aesthetics.",

                ImageUrl = https://momomats.s3.us-east-1.amazonaws.com/ChatGPT_Retro_Arcade.png,

                Price = 32.99m,

                CreatedAt = DateTimeOffset.UtcNow
            },

            new Mat
            {
                Name = "Midnight Garden",

                Description =
                    "A dark botanical pattern with glowing flowers and moonlit leaves.",

                Provider = "OpenAI",

                GenerationPrompt =
                    "Create an elegant dark botanical doormat with glowing flowers, moonlit leaves, subtle blue and purple tones, and intricate natural patterns.",

                ImageUrl = https://momomats.s3.us-east-1.amazonaws.com/ChatGPT_Midnight_Garden.png,

                Price = 35.99m,

                CreatedAt = DateTimeOffset.UtcNow
            },


            // =====================================================
            // GEMINI MATS
            // =====================================================

            new Mat
            {
                Name = "Botanical Geometry",

                Description =
                    "A modern combination of geometric shapes and botanical elements.",

                Provider = "Gemini",

                GenerationPrompt =
                    "Create a modern decorative doormat combining clean geometric shapes with botanical leaves and natural patterns.",

                ImageUrl = https://momomats.s3.us-east-1.amazonaws.com/Gemini_Botantical_Geometry.png,

                Price = 30.99m,

                CreatedAt = DateTimeOffset.UtcNow
            },

            new Mat
            {
                Name = "Ocean Mosaic",

                Description =
                    "An abstract ocean design featuring layered blue mosaic patterns.",

                Provider = "Gemini",

                GenerationPrompt =
                    "Create an abstract ocean-themed doormat using layered blue mosaic patterns, flowing waves, and geometric tile textures.",

                ImageUrl = https://momomats.s3.us-east-1.amazonaws.com/Gemini_Ocean_Mosaic.png,

                Price = 33.99m,

                CreatedAt = DateTimeOffset.UtcNow
            },

            new Mat
            {
                Name = "Desert Horizon",

                Description =
                    "A warm minimalist desert scene with dunes and a setting sun.",

                Provider = "Gemini",

                GenerationPrompt =
                    "Create a minimalist desert doormat design with flowing sand dunes, warm earth tones, and a large setting sun on the horizon.",

                ImageUrl = https://momomats.s3.us-east-1.amazonaws.com/Gemini_Desert_Horizon.png,

                Price = 29.99m,

                CreatedAt = DateTimeOffset.UtcNow
            },

            new Mat
            {
                Name = "Forest Path",

                Description =
                    "A quiet illustrated forest trail surrounded by deep green foliage.",

                Provider = "Gemini",

                GenerationPrompt =
                    "Create an illustrated decorative doormat showing a peaceful forest trail surrounded by rich green foliage and soft natural lighting.",

                ImageUrl = https://momomats.s3.us-east-1.amazonaws.com/Gemini_Forest_Path.png,

                Price = 34.99m,

                CreatedAt = DateTimeOffset.UtcNow
            },

            new Mat
            {
                Name = "Abstract Waves",

                Description =
                    "A contemporary flowing pattern inspired by colorful ocean currents.",

                Provider = "Gemini",

                GenerationPrompt =
                    "Create a contemporary abstract doormat featuring flowing colorful waves inspired by ocean currents and modern graphic design.",

                ImageUrl = https://momomats.s3.us-east-1.amazonaws.com/Gemini_Abstract_Waves.png,

                Price = 31.99m,

                CreatedAt = DateTimeOffset.UtcNow
            }
        };

        await dbContext.Mats.AddRangeAsync(mats);

        await dbContext.SaveChangesAsync();
    }
}