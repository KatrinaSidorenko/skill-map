using System.Text.Json;
using System.Text.Json.Serialization;

using LearningPlatform.Workspace.WebSockets.Contracts;
using LearningPlatform.Workspace.WebSockets.Contracts.Commands;

public sealed class WorkspaceActionJsonConverter : JsonConverter<WorkspaceAction>
{
    public override WorkspaceAction Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        var workspaceId = GetPropertyCaseInsensitive(root, nameof(WorkspaceAction.WorkspaceId)).GetInt64();
        var actionType = (WorkspaceActionType)GetPropertyCaseInsensitive(root, nameof(WorkspaceAction.ActionType)).GetInt32();
        var payloadElement = GetPropertyCaseInsensitive(root, nameof(WorkspaceAction.Payload));

        IWorkspaceActionCommand payload = actionType switch
        {
            WorkspaceActionType.CreateLearningItem => payloadElement.Deserialize<AddLearningItemActionCommand>(options)!,
            WorkspaceActionType.UpdateLearningItem => payloadElement.Deserialize<UpdateLearningItemActionCommand>(options)!,
            WorkspaceActionType.DeleteLearningItem => payloadElement.Deserialize<DeleteLearningItemActionCommand>(options)!,
            WorkspaceActionType.CreateConnection => payloadElement.Deserialize<CreateLearningItemConnectionActionCommand>(options)!,
            WorkspaceActionType.DeleteConnection => payloadElement.Deserialize<DeleteLearningItemConnectionActionCommand>(options)!,
            _ => throw new JsonException($"Unknown WorkspaceActionType: {actionType}")
        };

        return new WorkspaceAction(workspaceId, actionType, payload);
    }

    public override void Write(Utf8JsonWriter writer, WorkspaceAction value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        var namingPolicy = options.PropertyNamingPolicy;

        writer.WriteNumber(namingPolicy?.ConvertName(nameof(WorkspaceAction.WorkspaceId)) ?? nameof(WorkspaceAction.WorkspaceId), value.WorkspaceId);
        writer.WriteNumber(namingPolicy?.ConvertName(nameof(WorkspaceAction.ActionType)) ?? nameof(WorkspaceAction.ActionType), (int)value.ActionType);

        writer.WritePropertyName(namingPolicy?.ConvertName(nameof(WorkspaceAction.Payload)) ?? nameof(WorkspaceAction.Payload));
        JsonSerializer.Serialize(writer, value.Payload, value.Payload.GetType(), options);

        writer.WriteEndObject();
    }

    private JsonElement GetPropertyCaseInsensitive(JsonElement element, string name)
    {
        if (element.TryGetProperty(name, out var prop)) return prop;
        // Fallback to lowercase first letter (camelCase)
        string camelCase = char.ToLower(name[0]) + name.Substring(1);
        if (element.TryGetProperty(camelCase, out prop)) return prop;

        throw new JsonException($"Property {name} not found.");
    }
}