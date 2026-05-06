namespace SkillMap.Shared.Options;
public class PasswordHashOptions
{
    public const string SectionName = "PasswordHashOptions";

    public int MemorySize { get; set; } 
    public int Iterations { get; set; }
    public int DegreeOfParallelism { get; set; }
    public int SaltSize { get; set; }
    public int HashSize { get; set; }
    public string Separator { get; set; } = ":";
}
