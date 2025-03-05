package model

import (
	"beauty-server/internal/domain/entity"
	"beauty-server/internal/domain/value_object"
	"github.com/google/uuid"
	"time"
)

type ServiceModel struct {
	ID             uuid.UUID `gorm:"type:uuid;primaryKey"`
	OrganizationID uuid.UUID `gorm:"type:uuid;not null"`
	Name           string    `gorm:"type:varchar(255);not null"`
	Description    *string   `gorm:"type:varchar(1000);null"`
	Duration       *int      `gorm:"type:int;null"`
	Price          *float64  `gorm:"type:numeric(10,2);null"`

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

	var duration *time.Duration
	if m.Duration != nil {
		dur := time.Duration(*m.Duration) * time.Minute
		duration = &dur
	}

	var price *value_object.ServicePrice
	if m.Price != nil {
		p, err := value_object.NewServicePrice(*m.Price)
		if err != nil {
			return nil, err
		}
		price = &p
	}

	return &entity.Service{
		Id:             m.ID,
		OrganizationId: m.OrganizationID,
		Name:           name,
		Description:    description,
		Duration:       duration,
		Price:          price,
	}, nil
}

func FromDomainService(service *entity.Service) *ServiceModel {
	var description *string
	if service.Description != nil {
		desc := service.Description.Value()
		description = &desc
	}

	var duration *int
	if service.Duration != nil {
		dur := int(service.Duration.Minutes())
		duration = &dur
	}

	var price *float64
	if service.Price != nil {
		p := service.Price.Value()
		price = &p
	}

	return &ServiceModel{
		ID:             service.Id,
		OrganizationID: service.OrganizationId,
		Name:           service.Name.Value(),
		Description:    description,
		Duration:       duration,
		Price:          price,
	}
}
