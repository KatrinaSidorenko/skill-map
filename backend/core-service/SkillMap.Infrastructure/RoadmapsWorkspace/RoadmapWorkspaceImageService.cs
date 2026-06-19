using SkillMap.Business.RoadmapsWorkspace;
using SkillMap.Infrastructure.Files;

namespace SkillMap.Infrastructure.RoadmapsWorkspace;
internal class RoadmapWorkspaceImageService : ImageFileService, IRoadmapWorkspaceImagesService
{
    private const string _containerName = "workspaces-cover-images";
    public RoadmapWorkspaceImageService(IBlobStorageService blobStorage) : base(blobStorage, _containerName) { }
}
