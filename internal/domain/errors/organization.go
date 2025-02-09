package errors

import (
	"fmt"
	"github.com/google/uuid"
)

const (
	ErrOrganizationNameTooShort = "organization name must be at least %d characters long"
	ErrOrganizationNameTooLong  = "organization name must not exceed %d characters"

	ErrOrganizationDescriptionTooShort = "organization description must be at least %d characters long"
	ErrOrganizationDescriptionTooLong  = "organization description must not exceed %d characters"

	ErrOrganizationColorInvalidFormat = "organization color must be a valid HEX format (e.g. #RRGGBB)"
	ErrOrganizationPhotoInvalidFormat = "organization photo URL cannot be empty or invalid"

	ErrOrganizationVenueAlreadyExists = "venue id %s already exists in the organization"
	ErrOrganizationVenueNotFound      = "venue id %s not found in the organization"

	ErrOrganizationNotFound = "organization id %s not found"
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

func NewErrOrganizationVenueAlreadyExists(venueId uuid.UUID) *CustomError {
	return Conflict(fmt.Sprintf(ErrOrganizationVenueAlreadyExists, venueId.String()))
}

func NewErrOrganizationVenueNotFound(venueId uuid.UUID) *CustomError {
	return NotFound(fmt.Sprintf(ErrOrganizationVenueNotFound, venueId.String()))
}

func NewErrErrOrganizationNotFound(organizationId uuid.UUID) *CustomError {
	return NotFound(fmt.Sprintf(ErrOrganizationNotFound, organizationId.String()))
}

func NewErrOrganizationColorInvalidFormat() *CustomError {
	return Validation(ErrOrganizationColorInvalidFormat)
}

func NewErrOrganizationPhotoInvalidFormat() *CustomError {
	return Validation(ErrOrganizationPhotoInvalidFormat)
}
