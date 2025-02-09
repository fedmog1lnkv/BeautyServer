package entity

import (
	"beauty-server/internal/domain/enum"
	"beauty-server/internal/domain/errors"
	"beauty-server/internal/domain/value_object"
	"github.com/google/uuid"
	"os"
)

type Organization struct {
	Id           uuid.UUID
	Name         value_object.OrganizationName
	Description  *value_object.OrganizationDescription
	Subscription enum.OrganizationSubscription
	Theme        value_object.OrganizationThemeConfig
	VenueIds     []uuid.UUID
}

func NewOrganization(id uuid.UUID, name string) (*Organization, error) {
	organizationName, err := value_object.NewOrganizationName(name)
	if err != nil {
		return nil, err
	}

	orgTheme, err := value_object.NewOrganizationThemeConfig(os.Getenv("DEFAULT_ORGANIZATION_COLOR"), nil)
	if err != nil {
		return nil, err
	}

	return &Organization{
		Id:           id,
		Name:         organizationName,
		Description:  nil,
		Subscription: enum.Disabled,
		Theme:        orgTheme,
		VenueIds:     []uuid.UUID{},
	}, nil
}

func (o *Organization) UpdateName(name string) error {
	organizationName, err := value_object.NewOrganizationName(name)
	if err != nil {
		return err
	}

	if o.Name.Equal(organizationName) {
		return nil
	}

	o.Name = organizationName
	return nil
}

func (o *Organization) UpdateDescription(description string) error {
	organizationDescription, err := value_object.NewOrganizationDescription(description)
	if err != nil {
		return err
	}

	if o.Description != nil && o.Description.Equal(organizationDescription) {
		return nil
	}

	o.Description = &organizationDescription
	return nil
}

func (o *Organization) UpdateSubscription(subscription enum.OrganizationSubscription) {
	o.Subscription = subscription
}

func (o *Organization) UpdateColor(color string) error {
	return o.UpdateTheme(color, o.Theme.GetPhoto())
}

func (o *Organization) UpdatePhoto(photo *string) error {
	return o.UpdateTheme(o.Theme.GetColor(), photo)
}

func (o *Organization) UpdateTheme(color string, photo *string) error {
	theme, err := value_object.NewOrganizationThemeConfig(color, photo)
	if err != nil {
		return err
	}

	if o.Theme.Equal(theme) {
		return nil
	}

	o.Theme = theme
	return nil
}

func (o *Organization) AddVenueId(venueId uuid.UUID) error {
	for _, id := range o.VenueIds {
		if id == venueId {
			return errors.NewErrOrganizationVenueAlreadyExists(venueId)
		}
	}

	o.VenueIds = append(o.VenueIds, venueId)
	return nil
}

func (o *Organization) RemoveVenueId(venueId uuid.UUID) error {
	index := -1
	for i, id := range o.VenueIds {
		if id == venueId {
			index = i
			break
		}
	}

	if index == -1 {
		return errors.NewErrOrganizationVenueNotFound(venueId)
	}

	o.VenueIds = append(o.VenueIds[:index], o.VenueIds[index+1:]...)
	return nil
}
