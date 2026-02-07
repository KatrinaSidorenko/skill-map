// <copyright file="EmailOptions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SkillMap.Shared.Options;

public class EmailOptions
{
    public const string SectionName = "EmailConfig";

    public string SmtpServer { get; set; } = string.Empty;

    public int SmtpPort { get; set; } = 587;

    public string SmtpUser { get; set; } = string.Empty;

    public string SmtpPass { get; set; } = string.Empty;

    public string FromEmail { get; set; } = string.Empty;

    public string FromName { get; set; } = string.Empty;
}