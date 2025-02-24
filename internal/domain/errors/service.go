package errors

import "fmt"

const (
	ErrServiceNameTooShort = "service name must be at least %d characters long"
	ErrServiceNameTooLong  = "service name must not exceed %d characters"

	ErrServiceDescriptionTooShort = "service description must be at least %d characters long"
	ErrServiceDescriptionTooLong  = "service description must not exceed %d characters"
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
