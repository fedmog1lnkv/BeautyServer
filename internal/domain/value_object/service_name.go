package value_object

import (
	"beauty-server/internal/domain/errors"
	"strings"
)

const (
	ServiceNameMinLength = 3
	ServiceNameMaxLength = 50
)

type ServiceName struct {
	value string
}

func NewServiceName(name string) (ServiceName, error) {
	name = strings.TrimSpace(name)
	if len(name) < ServiceNameMinLength {
		return ServiceName{}, errors.NewErrServiceNameTooShort(ServiceNameMinLength)
	}
	if len(name) > ServiceNameMaxLength {
		return ServiceName{}, errors.NewErrServiceNameTooLong(ServiceNameMaxLength)
	}
	return ServiceName{value: name}, nil
}

func (u ServiceName) Value() string {
	return u.value
}

func (u ServiceName) Equal(other ServiceName) bool {
	return u.value == other.value
}
