using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillMap.Shared.Results;
public class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException(string resourceName, string resourceId)
        : base($"Resource '{resourceName}' with ID '{resourceId}' was not found.")
    {
    }
}