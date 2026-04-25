using System.Text.Json;
using System.Text.Json.Serialization;

using LearningPlatform.Workspace.WebSockets.Contracts.Commands;

namespace LearningPlatform.Workspace.WebSockets.Contracts.Serialization;

public sealed class WorkspaceActionJsonConverter : JsonConverter<WorkspaceAction>
{
    public override WorkspaceAction Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        var workspaceId = root.GetProperty(nameof(WorkspaceAction.WorkspaceId)).GetInt64();
        var actionType = (WorkspaceActionType)root.GetProperty(nameof(WorkspaceAction.ActionType)).GetInt32();
        var payloadElement = root.GetProperty(nameof(WorkspaceAction.Payload));

        IWorkspaceActionCommand payload = actionType switch
        {
            WorkspaceActionType.CreateLearningItem =>
            payloadElement.Deserialize<AddLearningItemActionCommand>(options)!,
            WorkspaceActionType.UpdateLearningItem =>
          payloadElement.Deserialize<UpdateLearningItemActionCommand>(options)!,
            WorkspaceActionType.DeleteLearningItem =>
                      payloadElement.Deserialize<DeleteLearningItemActionCommand>(options)!,
            WorkspaceActionType.CreateConnection =>
            payloadElement.Deserialize<CreateLearningItemConnectionActionCommand>(options)!,
            WorkspaceActionType.DeleteConnection =>
                  payloadElement.Deserialize<DeleteLearningItemConnectionActionCommand>(options)!,
            _ => throw new JsonException($"Unknown WorkspaceActionType: {actionType}")
        };

        return new WorkspaceAction(workspaceId, actionType, payload);
    }

    public override void Write(Utf8JsonWriter writer, WorkspaceAction value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, options);
}
