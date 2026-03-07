using MediatR;

namespace SkillMap.Shared.EventBus;
public interface IIntegrationEvent : INotification
{
    Guid Id { get; }
    DateTimeOffset OccurredDateTime { get; }
}