using Microsoft.AspNetCore.Http;

namespace SkillMap.Shared.Files;

public record FileDefenition(string Extension, string ContentType, byte[] Signature);
public class FilesValidator
{
    private readonly List<FileDefenition> _allowedFiles;
    private readonly long _maxSizeBytes;

    public FilesValidator(List<FileDefenition> allowedFiles, long maxSizeBytes)
    {
        _allowedFiles = allowedFiles;
        _maxSizeBytes = maxSizeBytes;
    }

    // todo: can be written as builder
    public bool IsValid(IFormFile file, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (file == null || file.Length == 0)
        {
            errorMessage = "File is empty or missing.";
            return false;
        }

        if (file.Length > _maxSizeBytes)
        {
            errorMessage = $"File size exceeds the allowed limit of {_maxSizeBytes / 1024 / 1024} MB.";
            return false;
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var matchedDefinition = _allowedFiles.FirstOrDefault(f => f.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase));

        if (matchedDefinition == null)
        {
            errorMessage = $"The file extension '{extension}' is not allowed.";
            return false;
        }

        if (!file.ContentType.Equals(matchedDefinition.ContentType, StringComparison.OrdinalIgnoreCase))
        {
            errorMessage = "MIME type mismatch.";
            return false;
        }

        int signatureLength = matchedDefinition.Signature.Length;
        if (signatureLength > 0)
        {
            using var stream = file.OpenReadStream();
            byte[] fileHeader = new byte[signatureLength];

            int bytesRead = stream.Read(fileHeader, 0, signatureLength);

            if (bytesRead < signatureLength || !fileHeader.SequenceEqual(matchedDefinition.Signature))
            {
                errorMessage = "Security check failed: File content does not match its extension.";
                return false;
            }
        }

        return true;
    }
}


public static class FilesValidatorExtensions
{
    private static FileDefenition JpgFileDefinition = new FileDefenition(".jpg", "image/jpeg", new byte[] { 0xFF, 0xD8, 0xFF });
    private static FileDefenition JpegFileDefinition = new FileDefenition(".jpeg", "image/jpeg", new byte[] { 0xFF, 0xD8, 0xFF });
    private static FileDefenition PngFileDefinition = new FileDefenition(".png", "image/png", new byte[] { 0x89, 0x50, 0x4E, 0x47 });

    public static List<FileDefenition> GetImageFileDefinitions()
        => [
            JpgFileDefinition,
            JpegFileDefinition,
            PngFileDefinition
        ];

    public static long GetMaxImageSize() => 5 * 1024 * 1024;

    public static FilesValidator CreateImageFilesValidator() => new FilesValidator(GetImageFileDefinitions(), GetMaxImageSize());

}