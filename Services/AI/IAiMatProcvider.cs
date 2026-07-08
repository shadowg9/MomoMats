public interface IAiMatProvider
{
    Task<GeneratedMatResult> GenerateMatAsync(
        string prompt,
        CancellationToken cancellationToken = default);
}

public class GeneratedMatResult
{
}