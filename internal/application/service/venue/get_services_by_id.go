package venue

import (
	"beauty-server/internal/domain/entity"
	"fmt"
	"github.com/google/uuid"
)

func (s *VenueService) GetServicesById(venueId uuid.UUID) ([]*entity.Service, error) {
	// TODO : change to venue id, when timeslots
	services, err := s.serviceRepo.GetByOrganizationId(venueId)
	if err != nil {
		return nil, fmt.Errorf("failed to get services by venue id: %w", err)
	}
	return services, nil
}
