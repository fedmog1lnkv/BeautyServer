package value_object

import (
	"beauty-server/internal/domain/errors"
	"strings"
)

const (
	VenueNameMinLength = 3
	VenueNameMaxLength = 50
)

type VenueName struct {
	value string
}

func NewVenueName(name string) (VenueName, error) {
	name = strings.TrimSpace(name)
	if len(name) < VenueNameMinLength {
		return VenueName{}, errors.NewErrVenueNameTooShort(VenueNameMinLength)
	}
	if len(name) > VenueNameMaxLength {
		return VenueName{}, errors.NewErrVenueNameTooLong(VenueNameMaxLength)
	}
	return VenueName{value: name}, nil
}

func (u VenueName) Value() string {
	return u.value
}

func (u VenueName) Equal(other VenueName) bool {
	return u.value == other.value
}
