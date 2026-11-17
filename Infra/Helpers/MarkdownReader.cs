namespace Infra.Helpers;

public interface IMarkdownReader
{
    Task<string> ReadAsync(CancellationToken ct);
}

public class MarkdownReader
{
    private readonly string _filePath;

    public MarkdownReader(string basePath, string fileName)
    {
        _filePath = Path.Combine(basePath, fileName);
    }

    public async Task<string> ReadAsync(CancellationToken ct)
    {
        return await File.ReadAllTextAsync(_filePath, ct);
    }
}