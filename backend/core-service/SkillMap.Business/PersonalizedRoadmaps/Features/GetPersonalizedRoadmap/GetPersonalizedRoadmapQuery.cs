using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillMap.Business.PersonalizedRoadmaps.Features.GetPersonalizedRoadmap;
public record GetPersonalizedRoadmapQuery(long UserRoadmapId) : ICommand<PersonalizedRoadmapDto>;
