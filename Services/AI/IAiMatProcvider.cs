namespace MomoMats.Services.AI;


// ---------------------------------------------------------
// COMMON AI MAT PROVIDER CONTRACT
// ---------------------------------------------------------

public interface IAiMatProvider
{
    string ProviderName { get; }


    Task<GeneratedMatResult> GenerateMatAsync(
        string prompt,
        CancellationToken cancellationToken = default);
}


// ---------------------------------------------------------
// COMMON RESULT RETURNED BY ALL AI PROVIDERS
// ---------------------------------------------------------

public sealed record GeneratedMatResult(
    string Provider,
    byte[] ImageBytes,
    string ContentType);