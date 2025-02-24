package value_object

import (
	"beauty-server/internal/domain/errors"
	"strings"
)

const (
	ServiceDescriptionMinLength = 10
	ServiceDescriptionMaxLength = 500
)

type ServiceDescription struct {
	value string
}

func NewServiceDescription(name string) (ServiceDescription, error) {
	name = strings.TrimSpace(name)
	if len(name) < ServiceDescriptionMinLength {
		return ServiceDescription{}, errors.NewErrServiceDescriptionTooShort(ServiceDescriptionMinLength)
	}
	if len(name) > ServiceDescriptionMaxLength {
		return ServiceDescription{}, errors.NewErrServiceDescriptionTooLong(ServiceDescriptionMaxLength)
	}
	return ServiceDescription{value: name}, nil
}

func (u ServiceDescription) Value() string {
	return u.value
}

func (u ServiceDescription) Equal(other ServiceDescription) bool {
	return u.value == other.value
}
