// <copyright file="DatabaseConnectionOptions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SkillMap.Shared.Options;

public class DatabaseConnectionOptions
{
    public const string SectionName = "DatabaseConnection";

    public string Server { get; set; } = string.Empty;

    public string Database { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public int Port { get; set; } = 5432;
}