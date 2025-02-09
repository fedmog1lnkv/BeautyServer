package eventbus

import (
	"beauty-server/internal/domain/events"
	"fmt"
	"github.com/asaskevich/EventBus"
)

type EventBusWrapper struct {
	bus EventBus.Bus
}

func NewEventBusWrapper() *EventBusWrapper {
	return &EventBusWrapper{
		bus: EventBus.New(),
	}
}

func (bus *EventBusWrapper) Subscribe(eventType string, handler func(event events.Event)) error {
	err := bus.bus.Subscribe(eventType, handler)
	if err != nil {
		return fmt.Errorf("failed to subscribe to event %s: %w", eventType, err)
	}
	return nil
}

func (bus *EventBusWrapper) Publish(event events.Event) error {
	eventType := event.Type()
	bus.bus.Publish(eventType, event)
	return nil
}
