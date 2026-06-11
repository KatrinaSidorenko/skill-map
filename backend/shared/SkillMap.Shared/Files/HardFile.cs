using Microsoft.AspNetCore.Http;

namespace SkillMap.Shared.Files;

// supposed that will be passed already normalized and valid filename and type
public record HardFile(string FileName, byte[] Content, string ContentType);

public static class HardFileExtensions
{
    public static async Task<HardFile?> ToHardFileAsync(this IFormFile formFile, CancellationToken ct)
    {
        if (formFile == null || formFile.Length == 0)
        {
            return null;
        }
        using var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream, ct);
        return new HardFile(formFile.FileName, memoryStream.ToArray(), formFile.ContentType);
    }
}
