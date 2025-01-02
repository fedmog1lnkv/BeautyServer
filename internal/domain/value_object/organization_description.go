package value_object

import (
	"beauty-server/internal/domain/errors"
	"strings"
)

const (
	OrganizationDescriptionMinLength = 10
	OrganizationDescriptionMaxLength = 500
)

type OrganizationDescription struct {
	value string
}

func NewOrganizationDescription(name string) (OrganizationDescription, error) {
	name = strings.TrimSpace(name)
	if len(name) < OrganizationDescriptionMinLength {
		return OrganizationDescription{}, errors.NewErrOrganizationDescriptionTooShort(OrganizationDescriptionMinLength)
	}
	if len(name) > OrganizationDescriptionMaxLength {
		return OrganizationDescription{}, errors.NewErrOrganizationDescriptionTooLong(OrganizationDescriptionMaxLength)
	}
	return OrganizationDescription{value: name}, nil
}

func (u OrganizationDescription) Value() string {
	return u.value
}

func (u OrganizationDescription) Equal(other OrganizationDescription) bool {
	return u.value == other.value
}
