using SkillMap.Api.UserAccount;
using SkillMap.Infrastructure.Files;

namespace SkillMap.Infrastructure.Account;
internal class ProfileImagesService : ImageFileService, IProfileImagesService
{
    private const string _containerName = "profile-images";
    public ProfileImagesService(IBlobStorageService blobStorage) : base(blobStorage, _containerName) { }
}
