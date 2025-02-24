package service

import (
	"beauty-server/internal/domain/entity"
	"github.com/google/uuid"
)

func (s *ServiceService) Create(organizationId uuid.UUID, name string) error {
	id := uuid.New()

	newService, err := entity.NewService(id, organizationId, name, s.organizationRepo)
	if err != nil {
		return err
	}

	err = s.serviceRepo.Save(newService)
	if err != nil {
		return err
	}

	return nil
}
