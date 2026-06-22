namespace SkillMap.Infrastructure.Files;

public class AzureBlobStorageOptions
{
    public const string SectionName = "AzureBlobStorage";
    public string ConnectionString { get; set; }
}
