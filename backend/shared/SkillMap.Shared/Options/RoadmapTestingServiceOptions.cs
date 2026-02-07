// <copyright file="RoadmapTestingServiceOptions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillMap.Shared.Options;

public class RoadmapTestingServiceOptions
{
    public const string SectionName = "RoadmapTestingService";

    public string BaseUrl { get; init; }
}