package repository

import (
	"beauty-server/internal/domain/entity"
	"beauty-server/internal/domain/enum"
	"github.com/google/uuid"
)

type OrganizationRepository interface {
	GetById(id uuid.UUID) (*entity.Organization, error)
	GetByIdWithVenues(id uuid.UUID) (*entity.Organization, error)
	GetAll(limit, offset int) ([]*entity.Organization, error)
	GetBySubscription(limit, offset int, subscriptionType enum.OrganizationSubscription) ([]*entity.Organization, error)
	Save(organization *entity.Organization) error
	Update(organization *entity.Organization) error
	Remove(organization *entity.Organization) error
	Exists(organizationId uuid.UUID) (bool, error)
}
