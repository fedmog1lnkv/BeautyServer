package entity

import (
	"beauty-server/internal/domain/enum"
	"beauty-server/internal/domain/value_object"
	"github.com/google/uuid"
)

type Organization struct {
	Id           uuid.UUID
	Name         value_object.OrganizationName
	Description  *value_object.OrganizationDescription
	Subscription enum.OrganizationSubscription
	Color        *value_object.OrganizationColor
	Photo        *value_object.OrganizationPhoto
}

func NewOrganization(id uuid.UUID, name string) (*Organization, error) {
	organizationName, err := value_object.NewOrganizationName(name)
	if err != nil {
		return nil, err
	}

	return &Organization{
		Id:           id,
		Name:         organizationName,
		Description:  nil,
		Subscription: enum.Disabled,
		Color:        nil,
		Photo:        nil,
	}, nil
}

func (o *Organization) UpdateName(name string) error {
	organizationName, err := value_object.NewOrganizationName(name)
	if err != nil {
		return err
	}
	o.Name = organizationName
	return nil
}

func (o *Organization) UpdateDescription(description string) error {
	organizationDescription, err := value_object.NewOrganizationDescription(description)
	if err != nil {
		return err
	}
	o.Description = &organizationDescription
	return nil
}

func (o *Organization) UpdateSubscription(subscription enum.OrganizationSubscription) {
	o.Subscription = subscription
}

func (o *Organization) UpdateColor(color int) error {
	organizationColor, err := value_object.NewOrganizationColor(color)
	if err != nil {
		return err
	}
	o.Color = &organizationColor
	return nil
}

func (o *Organization) UpdatePhoto(photo string) error {
	organizationPhoto, err := value_object.NewOrganizationPhoto(photo)
	if err != nil {
		return err
	}
	o.Photo = &organizationPhoto
	return nil
}
