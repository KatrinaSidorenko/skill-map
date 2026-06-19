using System.Runtime.InteropServices.Marshalling;

using SkillMap.Business.Abstractions.Files;
using SkillMap.Shared.Files;

namespace SkillMap.Infrastructure.Files;

internal abstract class ImageFileService : IImagesFileService
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly string _containerName;

    protected ImageFileService(IBlobStorageService blobStorage, string containerName)
    {
        _blobStorageService = blobStorage;
        _containerName = containerName;
        if (!IsValidContainerName(containerName))
            throw new ArgumentException($"The provided container name '{containerName}' is invalid. It must be between 3 and 63 characters, contain only lowercase letters, numbers, and hyphens, and cannot start or end with a hyphen.", nameof(containerName));
    }

    protected virtual async Task<bool> DeleteImageInnerAsync(string fileName, CancellationToken ct)
        => await _blobStorageService.DeleteFileAsync(_containerName, fileName, ct);
    public virtual async Task<bool> DeleteImageAsync(string relativePath, CancellationToken ct)
    {
        if (!IsImageBelongToContainer(relativePath))
            throw new ArgumentException($"The provided relative path '{relativePath}' does not belong to the container '{_containerName}'.", nameof(relativePath));
        return await DeleteImageInnerAsync(ExtractFileNameFromRelativePath(relativePath), ct);
    }
    protected virtual async Task<string> GetImageAbsoluteUriInnerAsync(string relativePath, CancellationToken ct)
        => await _blobStorageService.GetAbsoluteFileUriAsync(_containerName, ExtractFileNameFromRelativePath(relativePath), ct);
    protected virtual async Task<SaveImageResult> UploadImageInnerAsync(HardFile file, CancellationToken ct)
    {
        // maybe add some retries
        var blobUploadResult = await _blobStorageService.UploadFileAsync(_containerName, file.FileName, file.Content, file.ContentType, ct);
        return new SaveImageResult(blobUploadResult.RelativePath, blobUploadResult.AbsolutePath, blobUploadResult.Filename);
    }
    public virtual async Task<string> GetImageAbsoluteUriAsync(string relativePath, CancellationToken ct)
    {
        if (!IsImageBelongToContainer(relativePath))
            throw new ArgumentException($"The provided relative path '{relativePath}' does not belong to the container '{_containerName}'.", nameof(relativePath));
        return await GetImageAbsoluteUriInnerAsync(relativePath, ct);
    }
    public virtual async Task<SaveImageResult> UploadImageAsync(HardFile file, CancellationToken ct)
        => await UploadImageInnerAsync(file, ct);

    private bool IsImageBelongToContainer(string relativePath)
    {
        var parts = relativePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2)
            return false;
        return string.Equals(parts[0], _containerName, StringComparison.OrdinalIgnoreCase);
    }
    private bool IsValidContainerName(string containerName)
    {
        if (string.IsNullOrWhiteSpace(containerName))
            return false;
        if (containerName.Length < 3 || containerName.Length > 63)
            return false;
        if (!System.Text.RegularExpressions.Regex.IsMatch(containerName, @"^[a-z0-9]+(-[a-z0-9]+)*$"))
            return false;
        return true;
    }
    private string ExtractFileNameFromRelativePath(string relativePath)
    {
        var parts = relativePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2)
            throw new ArgumentException($"The provided relative path '{relativePath}' is invalid.", nameof(relativePath));
        return parts[1];
    }
}
