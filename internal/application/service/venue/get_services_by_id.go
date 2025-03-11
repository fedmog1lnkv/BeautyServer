package venue

import (
	"beauty-server/internal/domain/entity"
	"fmt"
	"github.com/google/uuid"
)

func (s *VenueService) GetServicesById(venueId uuid.UUID) ([]*entity.Service, error) {
	// TODO : change to venue id, when timeslots
	venue, err := s.venueRepo.GetById(venueId)
	if err != nil {
		return nil, err
	}

	services, err := s.serviceRepo.GetByOrganizationId(venue.OrganizationId)
	if err != nil {
		return nil, fmt.Errorf("failed to get services by venue id: %w", err)
	}
	return services, nil
}
