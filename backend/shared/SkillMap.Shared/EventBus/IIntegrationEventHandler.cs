using MediatR;

namespace SkillMap.Shared.EventBus;
public interface IIntegrationEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IIntegrationEvent
{
}