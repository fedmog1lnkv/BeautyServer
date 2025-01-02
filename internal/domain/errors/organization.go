package errors

import "fmt"

const (
	ErrOrganizationNameTooShort        = "organization name must be at least %d characters long"
	ErrOrganizationNameTooLong         = "organization name must not exceed %d characters"
	ErrOrganizationDescriptionTooShort = "organization description must be at least %d characters long"
	ErrOrganizationDescriptionTooLong  = "organization description must not exceed %d characters"
)

func NewErrOrganizationNameTooShort(minLength int) *CustomError {
	return Validation(fmt.Sprintf(ErrOrganizationNameTooShort, minLength))
}

func NewErrOrganizationNameTooLong(maxLength int) *CustomError {
	return Validation(fmt.Sprintf(ErrOrganizationNameTooLong, maxLength))
}

func NewErrOrganizationDescriptionTooShort(minLength int) *CustomError {
	return Validation(fmt.Sprintf(ErrOrganizationDescriptionTooShort, minLength))
}

func NewErrOrganizationDescriptionTooLong(maxLength int) *CustomError {
	return Validation(fmt.Sprintf(ErrOrganizationDescriptionTooLong, maxLength))
}
