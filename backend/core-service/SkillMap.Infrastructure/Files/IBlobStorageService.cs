namespace SkillMap.Infrastructure.Files;


internal record UploadBlobResult(string AbsolutePath, string RelativePath, string Filename);
internal interface IBlobStorageService
{
    Task<UploadBlobResult> UploadFileAsync(string containerName, string fileName, byte[] content, string mimeType, CancellationToken ct);
    Task<bool> DeleteFileAsync(string containerName, string fileName, CancellationToken ct);
    Task<string> GetAbsoluteFileUriAsync(string containerName, string fileName, CancellationToken ct);
}