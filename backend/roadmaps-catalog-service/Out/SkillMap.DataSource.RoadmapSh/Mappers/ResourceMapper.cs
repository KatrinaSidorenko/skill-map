using SkillMap.DataSource.RoadmapSh.Api;
using SkillMap.Shared.Models;

namespace SkillMap.DataSource.RoadmapSh.Mappers;

public static class ResourceMapper
{
    public static FileDataDto ToFile(this FileInfoResponse response)
    {
        return new FileDataDto
        {
            Name = response.Name,
            Type = response.Type,
            DownloadUrl = response.DownloadUrl,
            //Content = Convert.FromBase64String(response.Content),
        };
    }
}
