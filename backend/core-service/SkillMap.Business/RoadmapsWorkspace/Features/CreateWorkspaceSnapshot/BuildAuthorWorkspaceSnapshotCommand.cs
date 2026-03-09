using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillMap.Business.RoadmapsWorkspace.Features.CreateWorkspaceSnapshot;
public record BuildAuthorWorkspaceSnapshotCommand(long WorkspaceId, string RoadmapId) : ICommand<long>;
