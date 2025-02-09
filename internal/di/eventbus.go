package di

import (
	"beauty-server/internal/application/eventbus/handlers/venue"
	"beauty-server/internal/domain/events"
	"go.uber.org/fx"
	"log"
)

var EventBusContainer = fx.Provide(
	venue.NewOrganizationColorChangedEventHandler,
	venue.NewOrganizationPhotoChangedEventHandler,
)

func RegisterEventHandlers(eventBus events.EventBus,
	organizationColorChangedEventHandler *venue.OrganizationColorChangedEventHandler,
	organizationPhotoChangedEventHandler *venue.OrganizationPhotoChangedEventHandler) {

	err := eventBus.Subscribe("OrganizationColorChangedEvent", func(event events.Event) {
		log.Println("Event received: OrganizationColorChangedEvent")
		if err := organizationColorChangedEventHandler.Handle(event); err != nil {
			log.Printf("Error handling event: %v", err)
		}
	})

	err = eventBus.Subscribe("OrganizationPhotoChangedEvent", func(event events.Event) {
		log.Println("Event received: OrganizationPhotoChangedEvent")
		if err := organizationPhotoChangedEventHandler.Handle(event); err != nil {
			log.Printf("Error handling event: %v", err)
		}
	})

	if err != nil {
		log.Printf("Failed to subscribe: %v", err)
	} else {
		log.Println("Successfully subscribed to OrganizationColorChangedEvent")
	}

	log.Println("Application started and subscribing...")
}
