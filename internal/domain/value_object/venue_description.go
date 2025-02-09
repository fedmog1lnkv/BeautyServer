package value_object

import (
	"beauty-server/internal/domain/errors"
	"strings"
)

const (
	VenueDescriptionMinLength = 10
	VenueDescriptionMaxLength = 500
)

type VenueDescription struct {
	value string
}

func NewVenueDescription(name string) (VenueDescription, error) {
	name = strings.TrimSpace(name)
	if len(name) < VenueDescriptionMinLength {
		return VenueDescription{}, errors.NewErrVenueDescriptionTooShort(VenueDescriptionMinLength)
	}
	if len(name) > VenueDescriptionMaxLength {
		return VenueDescription{}, errors.NewErrVenueDescriptionTooLong(VenueDescriptionMaxLength)
	}
	return VenueDescription{value: name}, nil
}

func (u VenueDescription) Value() string {
	return u.value
}

func (u VenueDescription) Equal(other VenueDescription) bool {
	return u.value == other.value
}
