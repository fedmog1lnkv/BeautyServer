package errors

import (
	"fmt"
	"github.com/google/uuid"
)

const (
	ErrServiceNameTooShort = "service name must be at least %d characters long"
	ErrServiceNameTooLong  = "service name must not exceed %d characters"

	ErrServiceDescriptionTooShort = "service description must be at least %d characters long"
	ErrServiceDescriptionTooLong  = "service description must not exceed %d characters"

	ErrServiceNotFound = "service id %s not found"

	ErrServicePriceInvalid = "service price must be greater than zero"
)

func NewErrServiceNameTooShort(minLength int) *CustomError {
	return Validation(fmt.Sprintf(ErrServiceNameTooShort, minLength))
}

func NewErrServiceNameTooLong(maxLength int) *CustomError {
	return Validation(fmt.Sprintf(ErrServiceNameTooLong, maxLength))
}

func NewErrServiceDescriptionTooShort(minLength int) *CustomError {
	return Validation(fmt.Sprintf(ErrServiceDescriptionTooShort, minLength))
}

func NewErrServiceDescriptionTooLong(maxLength int) *CustomError {
	return Validation(fmt.Sprintf(ErrServiceDescriptionTooLong, maxLength))
}

func NewErrServiceNotFound(serviceId uuid.UUID) *CustomError {
	return NotFound(fmt.Sprintf(ErrServiceNotFound, serviceId.String()))
}

func NewErrServicePriceInvalid() *CustomError {
	return Validation(ErrServicePriceInvalid)
}
