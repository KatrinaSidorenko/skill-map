// <copyright file="OptionsExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SkillMap.Shared.Options;

public static class OptionsExtensions
{
    public static string GetPostgresConnectionString(this DatabaseConnectionOptions options)
        => $"Server={options.Server};Port={options.Port};Database={options.Database};User Id={options.UserId};Password={options.Password};";
}