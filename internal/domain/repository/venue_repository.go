package repository

import (
	"beauty-server/internal/domain/entity"
	"beauty-server/internal/domain/value_object"
	"github.com/google/uuid"
)

type VenueRepository interface {
	GetById(id uuid.UUID) (*entity.Venue, error)
	GetAll(limit, offset int) ([]*entity.Venue, error)
	GetByLocation(location value_object.Location, limit, offset int) ([]*entity.Venue, error)
	GetByOrganizationId(organizationId uuid.UUID) ([]*entity.Venue, error)
	Save(venue *entity.Venue) error
	Update(venue *entity.Venue) error
	Remove(venue *entity.Venue) error
}
