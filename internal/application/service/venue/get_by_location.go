package venue

import (
	"beauty-server/internal/domain/entity"
	"beauty-server/internal/domain/value_object"
	"fmt"
)

func (s *VenueService) GetByLocation(latitude, longitude float64, limit, offset int) ([]*entity.Venue, error) {
	location := value_object.Location{
		Latitude:  latitude,
		Longitude: longitude,
	}
	venues, err := s.venueRepo.GetByLocation(location, limit, offset)
	if err != nil {
		return nil, fmt.Errorf("failed to get venues by location: %w", err)
	}
	return venues, nil
}
