package repository

import (
	"beauty-server/internal/domain/entity"
	"github.com/google/uuid"
)

type ServiceRepository interface {
	Save(service *entity.Service) error
	Update(service *entity.Service) error
	Remove(service *entity.Service) error
	GetByOrganizationId(organizationId uuid.UUID) ([]*entity.Service, error)
	ExistsService(id uuid.UUID) bool
}
