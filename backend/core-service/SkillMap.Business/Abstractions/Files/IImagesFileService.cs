using SkillMap.Shared.Files;

namespace SkillMap.Business.Abstractions.Files;
public interface IImagesFileService
{
    Task<bool> DeleteImageAsync(string relativePath, CancellationToken ct);
    Task<string> GetImageAbsoluteUriAsync(string relativePath, CancellationToken ct);
    Task<SaveImageResult> UploadImageAsync(HardFile file, CancellationToken ct);
}
