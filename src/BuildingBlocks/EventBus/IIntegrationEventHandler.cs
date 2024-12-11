﻿namespace EventBus
{
    public interface IIntegrationEventHandler<in TEvent> where TEvent : IntegrationEvent
    {
        Task Handle(TEvent @event);
    }
}