package organization

import (
	"beauty-server/internal/domain/events"
	"beauty-server/internal/domain/repository"
)

type OrganizationService struct {
	organizationRepo repository.OrganizationRepository
	venueRepo        repository.VenueRepository
	eventBus         events.EventBus
}

func NewOrganizationService(organizationRepo repository.OrganizationRepository, venueRepo repository.VenueRepository, eventBus events.EventBus) *OrganizationService {
	return &OrganizationService{
		organizationRepo: organizationRepo,
		venueRepo:        venueRepo,
		eventBus:         eventBus,
	}
}
