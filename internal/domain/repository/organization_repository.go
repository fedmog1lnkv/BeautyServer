package repository

import (
	"beauty-server/internal/domain/entity"
	"github.com/google/uuid"
)

type OrganizationRepository interface {
	GetById(id uuid.UUID) (*entity.Organization, error)
	GetAll(limit, offset int) ([]*entity.Organization, error)
	Save(organization *entity.Organization) error
	Update(organization *entity.Organization) error
	Remove(organization *entity.Organization) error
}
