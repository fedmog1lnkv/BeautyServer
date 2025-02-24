package entity

import (
	"beauty-server/internal/domain/errors"
	"beauty-server/internal/domain/value_object"
	"github.com/google/uuid"
)

type Service struct {
	Id             uuid.UUID
	OrganizationId uuid.UUID
	Name           value_object.ServiceName
	Description    *value_object.ServiceDescription
	Duration       *int // minutes // TODO : add value object
	Price          *float64
}

func NewService(id, organizationId uuid.UUID, name string, orgRepository OrganizationRepository) (*Service, error) {
	organization, err := orgRepository.GetById(organizationId)
	if err != nil {
		return nil, err
	}
	if organization == nil {
		return nil, errors.NewErrErrOrganizationNotFound(organizationId)
	}

	serviceName, err := value_object.NewServiceName(name)
	if err != nil {
		return nil, err
	}

	return &Service{
		Id:             id,
		OrganizationId: organizationId,
		Name:           serviceName,
	}, nil

}

func (s *Service) UpdateName(name string) error {
	serviceName, err := value_object.NewServiceName(name)
	if err != nil {
		return err
	}

	if s.Name.Equal(serviceName) {
		return nil
	}
	s.Name = serviceName
	return nil
}

func (s *Service) UpdateDescription(description string) error {
	desc, err := value_object.NewServiceDescription(description)
	if err != nil {
		return err
	}

	if s.Description != nil && s.Description.Equal(desc) {
		return nil
	}

	s.Description = &desc
	return nil
}

func (s *Service) UpdateDuration(duration int) error {
	// TODO : add check <= 0
	if s.Duration != nil && *s.Duration == duration {
		return nil
	}

	s.Duration = &duration
	return nil
}

func (s *Service) UpdatePrice(price float64) error {
	// TODO : add check <= 0
	if s.Price != nil && *s.Price == price {
		return nil
	}

	s.Price = &price
	return nil
}
