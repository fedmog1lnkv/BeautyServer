package entity

import (
	"beauty-server/internal/domain/errors"
	"beauty-server/internal/domain/value_object"
	"github.com/google/uuid"
)

type Venue struct {
	Id             uuid.UUID
	OrganizationId uuid.UUID
	Name           value_object.VenueName
	Description    *value_object.VenueDescription
	Theme          value_object.VenueThemeConfig
	Location       value_object.Location
}

type OrganizationRepository interface {
	Exists(organizationId uuid.UUID) (bool, error)
	GetById(id uuid.UUID) (*Organization, error)
}

func NewVenue(id, organizationId uuid.UUID, name string, latitude, longitude float64, orgRepository OrganizationRepository) (*Venue, error) {
	organization, err := orgRepository.GetById(organizationId)
	if err != nil {
		return nil, err
	}
	if organization == nil {
		return nil, errors.NewErrErrOrganizationNotFound(organizationId)
	}

	venueName, err := value_object.NewVenueName(name)
	if err != nil {
		return nil, err
	}

	venueLocation, err := value_object.NewLocation(latitude, longitude)
	if err != nil {
		return nil, err
	}

	venueTheme, err := value_object.NewVenueThemeConfig(organization.Theme.GetColor(), nil)
	if err != nil {
		return nil, err
	}

	return &Venue{
		Id:             id,
		OrganizationId: organizationId,
		Name:           venueName,
		Description:    nil,
		Theme:          venueTheme,
		Location:       venueLocation,
	}, nil
}

func (v *Venue) UpdateName(name string) error {
	venueName, err := value_object.NewVenueName(name)
	if err != nil {
		return err
	}

	if v.Name.Equal(venueName) {
		return nil
	}

	v.Name = venueName
	return nil
}

func (v *Venue) UpdateDescription(description string) error {
	venueDescription, err := value_object.NewVenueDescription(description)
	if err != nil {
		return err
	}

	if v.Description != nil && v.Description.Equal(venueDescription) {
		return nil
	}

	v.Description = &venueDescription
	return nil
}

func (v *Venue) UpdateColor(color string) error {
	return v.UpdateTheme(color, v.Theme.GetPhoto())
}

func (v *Venue) UpdatePhoto(photo *string) error {
	return v.UpdateTheme(v.Theme.GetColor(), photo)
}

func (v *Venue) UpdateTheme(color string, photo *string) error {
	theme, err := value_object.NewVenueThemeConfig(color, photo)
	if err != nil {
		return err
	}

	if v.Theme.Equal(theme) {
		return nil
	}

	v.Theme = theme
	return nil
}

func (v *Venue) UpdateLocation(latitude, longitude float64) error {
	venueLocation, err := value_object.NewLocation(latitude, longitude)
	if err != nil {
		return err
	}

	if v.Location.Equal(venueLocation) {
		return nil
	}

	v.Location = venueLocation
	return nil
}
