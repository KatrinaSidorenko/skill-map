using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SkillMap.Infrastructure.Files;
internal class AzureBlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<IBlobStorageService> _logger;
    public AzureBlobStorageService(IOptions<AzureBlobStorageOptions> options, ILogger<IBlobStorageService> logger)
    {
        _blobServiceClient = new BlobServiceClient(options.Value.ConnectionString); // todod: DI?
        _logger = logger;
    }

    // finame - name + .extension
    public async Task<bool> DeleteFileAsync(string containerName, string fileName, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(containerName))
            throw new ArgumentException("Container name cannot be null or whitespace.", nameof(containerName));
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be null or whitespace.", nameof(fileName));

        containerName = ToNormalizedContainerName(containerName);
        fileName = ToNormalizedFilename(fileName);

        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        try
        {
            var result = await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
            if (!result)
            {
                _logger.LogWarning("Blob {FileName} not found in container {ContainerName} for deletion", fileName, containerName);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file {FileName} from container {ContainerName}", fileName, containerName);
            throw new Exception($"Failed to delete file {fileName} from container {containerName}", ex);
        }
    }
    public async Task<string> GetAbsoluteFileUriAsync(string containerName, string fileName, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(containerName))
            throw new ArgumentException("Container name cannot be null or whitespace.", nameof(containerName));
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be null or whitespace.", nameof(fileName));

        containerName = ToNormalizedContainerName(containerName);
        fileName = ToNormalizedFilename(fileName);

        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await EnsureContainerExists(containerClient, ct);
        var blobClient = containerClient.GetBlobClient(fileName);

        try
        {
            if (await blobClient.ExistsAsync(ct))
            {
                return blobClient.Uri.AbsoluteUri;
            }
            else
            {
                _logger.LogWarning("Blob {FileName} not found in container {ContainerName}", fileName, containerName);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get URL for file {FileName} in container {ContainerName}", fileName, containerName);
            throw new Exception($"Failed to get URL for file {fileName} in container {containerName}", ex);
        }
    }
    public async Task<UploadBlobResult> UploadFileAsync(string containerName, string fileName, byte[] content, string contentType, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(containerName))
            throw new ArgumentException("Container name cannot be null or whitespace.", nameof(containerName));
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be null or whitespace.", nameof(fileName));
        if (content == null || content.Length == 0)
            throw new ArgumentException("Content cannot be null or empty.", nameof(content));

        containerName = ToNormalizedContainerName(containerName);
        fileName = ToNormalizedFilename(fileName);
        var uniqueFileName = CreateUniqueFilename(fileName);
        await EnsureContainerExists(_blobServiceClient.GetBlobContainerClient(containerName), ct);

        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(uniqueFileName);

        try
        {
            using var stream = new MemoryStream(content);
            var blobHttpOptions = new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = contentType } };
            await blobClient.UploadAsync(stream, blobHttpOptions, ct);
            return new UploadBlobResult(blobClient.Uri.AbsoluteUri, ComposeRelativePath(containerName, uniqueFileName), uniqueFileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file {FileName} to container {ContainerName}", uniqueFileName, containerName);
            throw new Exception($"Failed to upload file {uniqueFileName} to container {containerName}", ex);
        }
    }

    private async Task EnsureContainerExists(BlobContainerClient containerClient, CancellationToken ct) => await containerClient.CreateIfNotExistsAsync(publicAccessType: PublicAccessType.Blob, cancellationToken: ct);
    private string ComposeRelativePath(string containerName, string fileName) => $"{containerName}/{fileName}";
    private string CreateUniqueFilename(string filename) => $"{Guid.NewGuid().ToString("N")}_{filename}";

    private string ToNormalizedFilename(string filename) // todo: finish the logic
        => string.Concat(filename.Split(Path.GetInvalidFileNameChars()));
    private string ToNormalizedContainerName(string containerName) => containerName.ToLower().ToLower();
}
