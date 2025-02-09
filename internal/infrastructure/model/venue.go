package model

import (
	"beauty-server/internal/domain/entity"
	"beauty-server/internal/domain/value_object"
	"github.com/google/uuid"
)

type VenueModel struct {
	ID             uuid.UUID `gorm:"type:uuid;primaryKey"`
	OrganizationID uuid.UUID `gorm:"type:uuid;not null"`
	Name           string    `gorm:"type:varchar(255);not null"`
	Description    *string   `gorm:"type:varchar(1000);null"`

	Color string  `gorm:"type:varchar(7);null"`
	Photo *string `gorm:"type:varchar(2048);null"`

	Latitude  float64 `gorm:"type:decimal(9,6);not null"`
	Longitude float64 `gorm:"type:decimal(9,6);not null"`

	Organization OrganizationModel `gorm:"foreignKey:OrganizationID;references:Id"`
}

func (VenueModel) TableName() string {
	return "venues"
}

func (m *VenueModel) ToDomain() (*entity.Venue, error) {
	name, err := value_object.NewVenueName(m.Name)
	if err != nil {
		return nil, err
	}

	var description *value_object.VenueDescription
	if m.Description != nil {
		desc, err := value_object.NewVenueDescription(*m.Description)
		if err != nil {
			return nil, err
		}
		description = &desc
	}

	var theme value_object.VenueThemeConfig

	theme, err = value_object.NewVenueThemeConfig(m.Color, m.Photo)
	if err != nil {
		return nil, err
	}

	location, err := value_object.NewLocation(m.Latitude, m.Longitude)
	if err != nil {
		return nil, err
	}

	return &entity.Venue{
		Id:             m.ID,
		OrganizationId: m.OrganizationID,
		Name:           name,
		Description:    description,
		Theme:          theme,
		Location:       location,
	}, nil
}

func FromDomainVenue(venue *entity.Venue) *VenueModel {
	var description *string
	if venue.Description != nil {
		desc := venue.Description.Value()
		description = &desc
	}

	return &VenueModel{
		ID:             venue.Id,
		OrganizationID: venue.OrganizationId,
		Name:           venue.Name.Value(),
		Description:    description,
		Color:          venue.Theme.GetColor(),
		Photo:          venue.Theme.GetPhoto(),
		Latitude:       venue.Location.Latitude,
		Longitude:      venue.Location.Longitude,
	}
}
