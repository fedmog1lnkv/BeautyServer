package events

type EventBus interface {
	Publish(event Event) error
	Subscribe(eventType string, handler func(event Event)) error
}
