package venue

import (
	handlers "beauty-server/internal/application/eventbus/events/venue"
	domainEvents "beauty-server/internal/domain/events"
	"beauty-server/internal/domain/repository"
	"fmt"
)

type OrganizationPhotoChangedEventHandler struct {
	venueRepo repository.VenueRepository
}

func NewOrganizationPhotoChangedEventHandler(venueRepo repository.VenueRepository) *OrganizationPhotoChangedEventHandler {
	return &OrganizationPhotoChangedEventHandler{venueRepo: venueRepo}
}

func (h *OrganizationPhotoChangedEventHandler) Handle(event domainEvents.Event) error {
	PhotoChangedEvent, ok := event.(handlers.OrganizationPhotoChangedEvent)
	if !ok {
		return fmt.Errorf("invalid event type")
	}

	venues, err := h.venueRepo.GetByOrganizationId(PhotoChangedEvent.OrganizationID)
	if err != nil {
		return fmt.Errorf("failed to fetch venues: %w", err)
	}

	for _, venue := range venues {
		if *venue.Theme.GetPhoto() != *PhotoChangedEvent.OldPhoto {
			continue
		}

		if err := venue.UpdatePhoto(&PhotoChangedEvent.NewPhoto); err != nil {
			return fmt.Errorf("failed to update venue Photo: %w", err)
		}

		if err := h.venueRepo.Update(venue); err != nil {
			return fmt.Errorf("failed to save venue: %w", err)
		}
	}

	return nil
}
