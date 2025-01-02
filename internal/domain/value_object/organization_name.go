package value_object

import (
	"beauty-server/internal/domain/errors"
	"strings"
)

const (
	OrganizationNameMinLength = 3
	OrganizationNameMaxLength = 50
)

type OrganizationName struct {
	value string
}

func NewOrganizationName(name string) (OrganizationName, error) {
	name = strings.TrimSpace(name)
	if len(name) < OrganizationNameMinLength {
		return OrganizationName{}, errors.NewErrOrganizationNameTooShort(OrganizationNameMinLength)
	}
	if len(name) > OrganizationNameMaxLength {
		return OrganizationName{}, errors.NewErrOrganizationNameTooLong(OrganizationNameMaxLength)
	}
	return OrganizationName{value: name}, nil
}

func (u OrganizationName) Value() string {
	return u.value
}

func (u OrganizationName) Equal(other OrganizationName) bool {
	return u.value == other.value
}
