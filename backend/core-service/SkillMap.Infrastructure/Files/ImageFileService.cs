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
    }
    public virtual Task<bool> DeleteImageAsync(string fileName, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    protected virtual async Task<string> GetImageAbsoluteUriInnerAsync(string relativePath, CancellationToken ct)
    {
        var filename = relativePath.Split('/').LastOrDefault() ?? throw new ArgumentException($"The provided relative path '{relativePath}' is invalid.", nameof(relativePath));
        return await _blobStorageService.GetAbsoluteFileUriAsync(_containerName, filename, ct);
    }
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

    //private bool IsValidContainerName(string containerName)
    //{
    //    if (string.IsNullOrWhiteSpace(containerName))
    //        return false;
    //    if (containerName.Length < 3 || containerName.Length > 63)
    //        return false;
    //    if (!System.Text.RegularExpressions.Regex.IsMatch(containerName, @"^[a-z0-9]+(-[a-z0-9]+)*$"))
    //        return false;
    //    return true;
    //}
    //private bool IsValidFileName(string fileName)
    //{
    //    if (string.IsNullOrWhiteSpace(fileName))
    //        return false;
    //    if (fileName.Length > 255)
    //        return false;
    //    // Additional checks can be added here (e.g., allowed characters, extensions)
    //    return true;
    //}
}
