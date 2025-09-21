using System.Net;

namespace SkillMap.Shared.Results;

public record ExceptionResponse(HttpStatusCode StatusCode, string Description);
