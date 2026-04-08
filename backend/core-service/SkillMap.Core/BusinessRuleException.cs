using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillMap.Core;
public class BusinessRuleException : Exception
{
    public string RuleName { get; }
    public string Message { get; }

    public BusinessRuleException(string ruleName, string message) : base(message)
    {
        RuleName = ruleName;
        Message = message;
    }
    public BusinessRuleException(string ruleName) : this(ruleName, $"Business rule violated: {ruleName}")
    {
    }
}