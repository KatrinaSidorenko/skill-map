using Newtonsoft.Json;

namespace SkillMap.Shared.Models;

public class FileDataDto
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("download_url")]
    public string DownloadUrl { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("content")]
    public byte[] Content { get; set; }
}
