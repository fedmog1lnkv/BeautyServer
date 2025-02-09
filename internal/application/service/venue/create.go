package venue

import (
	"beauty-server/internal/domain/entity"
	"github.com/google/uuid"
)

func (s *VenueService) Create(organizationId uuid.UUID, name string, latitude, longitude float64) error {
	id := uuid.New()

	newVenue, err := entity.NewVenue(id, organizationId, name, latitude, longitude, s.organizationRepo)
	if err != nil {
		return err
	}

	organization, err := s.organizationRepo.GetById(organizationId)
	if err != nil {
		return err
	}

	if organization.Theme.GetPhoto() != nil {
		err = newVenue.UpdatePhoto(organization.Theme.GetPhoto())
		if err != nil {
			return err
		}
	}

	err = s.venueRepo.Save(newVenue)
	if err != nil {
		return err
	}

	return nil
}
