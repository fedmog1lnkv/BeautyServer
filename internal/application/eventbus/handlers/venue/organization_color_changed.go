package venue

import (
	handlers "beauty-server/internal/application/eventbus/events/venue"
	domainEvents "beauty-server/internal/domain/events"
	"beauty-server/internal/domain/repository"
	"fmt"
)

type OrganizationColorChangedEventHandler struct {
	venueRepo repository.VenueRepository
}

func NewOrganizationColorChangedEventHandler(venueRepo repository.VenueRepository) *OrganizationColorChangedEventHandler {
	return &OrganizationColorChangedEventHandler{venueRepo: venueRepo}
}

func (h *OrganizationColorChangedEventHandler) Handle(event domainEvents.Event) error {
	colorChangedEvent, ok := event.(handlers.OrganizationColorChangedEvent)
	if !ok {
		return fmt.Errorf("invalid event type")
	}

	venues, err := h.venueRepo.GetByOrganizationId(colorChangedEvent.OrganizationID)
	if err != nil {
		return fmt.Errorf("failed to fetch venues: %w", err)
	}

	for _, venue := range venues {
		if venue.Theme.GetColor() != colorChangedEvent.OldColor {
			continue
		}

		if err := venue.UpdateColor(colorChangedEvent.NewColor); err != nil {
			return fmt.Errorf("failed to update venue color: %w", err)
		}

		if err := h.venueRepo.Update(venue); err != nil {
			return fmt.Errorf("failed to save venue: %w", err)
		}
	}

	return nil
}
