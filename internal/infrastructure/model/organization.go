package model

import (
	"beauty-server/internal/domain/entity"
	"beauty-server/internal/domain/enum"
	"beauty-server/internal/domain/value_object"
	"github.com/google/uuid"
)

type OrganizationModel struct {
	ID           uuid.UUID                     `gorm:"type:uuid;primaryKey"`
	Name         string                        `gorm:"type:varchar(255);not null"`
	Description  *string                       `gorm:"type:varchar(1000);null"`
	Subscription enum.OrganizationSubscription `gorm:"type:organization_subscription;not null"`
	Color        *int                          `gorm:"type:int;null"`
	Photo        *string                       `gorm:"type:varchar(2048);null"`
}

func (OrganizationModel) TableName() string {
	return "organizations"
}

func (m *OrganizationModel) ToDomain() (*entity.Organization, error) {
	name, err := value_object.NewOrganizationName(m.Name)
	if err != nil {
		return nil, err
	}

	var description *value_object.OrganizationDescription
	if m.Description != nil {
		d, err := value_object.NewOrganizationDescription(*m.Description)
		if err != nil {
			return nil, err
		}
		description = &d
	}

	var color *value_object.OrganizationColor
	if m.Color != nil {
		c, err := value_object.NewOrganizationColor(*m.Color)
		if err != nil {
			return nil, err
		}
		color = &c
	}

	var photo *value_object.OrganizationPhoto
	if m.Photo != nil {
		p, err := value_object.NewOrganizationPhoto(*m.Photo)
		if err != nil {
			return nil, err
		}
		photo = &p
	}

	return &entity.Organization{
		Id:           m.ID,
		Name:         name,
		Description:  description,
		Subscription: m.Subscription,
		Color:        color,
		Photo:        photo,
	}, nil
}

func FromDomainOrganization(org *entity.Organization) *OrganizationModel {
	var description *string
	if org.Description != nil {
		desc := org.Description.Value()
		description = &desc
	}

	var color *int
	if org.Color != nil {
		colorVal := org.Color.Value()
		color = &colorVal
	}

	var photo *string
	if org.Photo != nil {
		photoVal := org.Photo.Value()
		photo = &photoVal
	}

	return &OrganizationModel{
		ID:           org.Id,
		Name:         org.Name.Value(),
		Description:  description,
		Subscription: org.Subscription,
		Color:        color,
		Photo:        photo,
	}
}
