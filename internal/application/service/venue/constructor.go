package venue

import (
	"beauty-server/internal/domain/repository"
)

type VenueService struct {
	venueRepo        repository.VenueRepository
	organizationRepo repository.OrganizationRepository
}

func NewVenueService(venueRepo repository.VenueRepository, organizationRepo repository.OrganizationRepository) *VenueService {
	return &VenueService{
		venueRepo:        venueRepo,
		organizationRepo: organizationRepo,
	}
}
