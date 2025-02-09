package model

import (
	"beauty-server/internal/domain/entity"
	"beauty-server/internal/domain/enum"
	"beauty-server/internal/domain/value_object"
	"github.com/google/uuid"
)

type OrganizationModel struct {
	Id           uuid.UUID                     `gorm:"type:uuid;primaryKey"`
	Name         string                        `gorm:"type:varchar(255);not null"`
	Description  *string                       `gorm:"type:varchar(1000);null"`
	Subscription enum.OrganizationSubscription `gorm:"type:organization_subscription;not null"`

	Color string  `gorm:"type:varchar(7);not null"`
	Photo *string `gorm:"type:varchar(2048);null"`

	VenueIds []uuid.UUID `gorm:"-"` // only for read
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

	var theme value_object.OrganizationThemeConfig

	theme, err = value_object.NewOrganizationThemeConfig(m.Color, m.Photo)
	if err != nil {
		return nil, err
	}

	return &entity.Organization{
		Id:           m.Id,
		Name:         name,
		Description:  description,
		Subscription: m.Subscription,
		Theme:        theme,
		VenueIds:     m.VenueIds,
	}, nil
}

func FromDomainOrganization(org *entity.Organization) *OrganizationModel {
	var description *string
	if org.Description != nil {
		desc := org.Description.Value()
		description = &desc
	}

	return &OrganizationModel{
		Id:           org.Id,
		Name:         org.Name.Value(),
		Description:  description,
		Subscription: org.Subscription,
		Color:        org.Theme.GetColor(),
		Photo:        org.Theme.GetPhoto(),
		VenueIds:     nil,
	}
}
