using SkillMap.Business.Abstractions.Files;

namespace SkillMap.Business.Helpers;
public static class FilesHelper
{
    public static async Task<string?> GetImageAbsoluteUriSafeAsync(this IImagesFileService imagesFileService, string? relativePath, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(relativePath))
            return string.Empty;
        if (IsAbsoluteUrl(relativePath)) // for old version links support
            return relativePath;
        return await imagesFileService.GetImageAbsoluteUriAsync(relativePath, cancellationToken);
    }

    private static bool IsAbsoluteUrl(string url) => Uri.TryCreate(url, UriKind.Absolute, out _);
}
