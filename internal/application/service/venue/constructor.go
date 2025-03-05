package venue

import (
	"beauty-server/internal/domain/repository"
)

type VenueService struct {
	venueRepo        repository.VenueRepository
	organizationRepo repository.OrganizationRepository
	serviceRepo      repository.ServiceRepository
}

func NewVenueService(venueRepo repository.VenueRepository, organizationRepo repository.OrganizationRepository, serviceRepo repository.ServiceRepository) *VenueService {
	return &VenueService{
		venueRepo:        venueRepo,
		organizationRepo: organizationRepo,
		serviceRepo:      serviceRepo,
	}
}
