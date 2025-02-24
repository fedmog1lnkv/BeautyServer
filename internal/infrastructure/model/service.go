package model

import (
	"beauty-server/internal/domain/entity"
	"beauty-server/internal/domain/value_object"
	"github.com/google/uuid"
)

type ServiceModel struct {
	ID             uuid.UUID `gorm:"type:uuid;primaryKey"`
	OrganizationID uuid.UUID `gorm:"type:uuid;not null"`
	Name           string    `gorm:"type:varchar(255);not null"`
	Description    *string   `gorm:"type:varchar(1000);null"`

	Organization OrganizationModel `gorm:"foreignKey:OrganizationID;references:Id"`
}

func (ServiceModel) TableName() string {
	return "services"
}

func (m *ServiceModel) ToDomain() (*entity.Service, error) {
	name, err := value_object.NewServiceName(m.Name)
	if err != nil {
		return nil, err
	}

	var description *value_object.ServiceDescription
	if m.Description != nil {
		desc, err := value_object.NewServiceDescription(*m.Description)
		if err != nil {
			return nil, err
		}
		description = &desc
	}

	return &entity.Service{
		Id:             m.ID,
		OrganizationId: m.OrganizationID,
		Name:           name,
		Description:    description,
	}, nil
}

func FromDomainService(service *entity.Service) *ServiceModel {
	var description *string
	if service.Description != nil {
		desc := service.Description.Value()
		description = &desc
	}

	return &ServiceModel{
		ID:             service.Id,
		OrganizationID: service.OrganizationId,
		Name:           service.Name.Value(),
		Description:    description,
	}
}
