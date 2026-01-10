using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillMap.Shared.Options;

public sealed class OpenAiOptions
{
    public const string SectionName = "OpenAi";
    public string ApiKey { get; init; }
    public string Model { get; init; }
    public int TimeoutSeconds { get; init; }
    public int? MaxOutputTokens { get; init; }
}
