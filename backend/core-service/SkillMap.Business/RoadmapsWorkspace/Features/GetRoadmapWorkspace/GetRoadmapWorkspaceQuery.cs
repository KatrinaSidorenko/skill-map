using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillMap.Business.PersonalizedRoadmaps.Features.GetPersonalizedRoadmap;
public record GetRoadmapWorkspaceQuery(long UserRoadmapId) : ICommand<RoadmapWorkspaceDto>;
