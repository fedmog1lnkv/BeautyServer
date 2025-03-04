package errors

import (
	"fmt"
	"github.com/google/uuid"
)

const (
	ErrVenueNameTooShort = "venue name must be at least %d characters long"
	ErrVenueNameTooLong  = "venue name must not exceed %d characters"

	ErrVenueDescriptionTooShort = "venue description must be at least %d characters long"
	ErrVenueDescriptionTooLong  = "venue description must not exceed %d characters"

	ErrVenueColorInvalidFormat = "organization color must be a valid HEX format (e.g. #RRGGBB)"

	ErrVenueNotFound = "venue id %s not found"
)

func NewErrVenueNameTooShort(minLength int) *CustomError {
	return Validation(fmt.Sprintf(ErrVenueNameTooShort, minLength))
}

func NewErrVenueNameTooLong(maxLength int) *CustomError {
	return Validation(fmt.Sprintf(ErrVenueNameTooLong, maxLength))
}

func NewErrVenueDescriptionTooShort(minLength int) *CustomError {
	return Validation(fmt.Sprintf(ErrVenueDescriptionTooShort, minLength))
}

func NewErrVenueDescriptionTooLong(maxLength int) *CustomError {
	return Validation(fmt.Sprintf(ErrVenueDescriptionTooLong, maxLength))
}

func NewErrVenueColorInvalidFormat() *CustomError {
	return Validation(ErrVenueColorInvalidFormat)
}

func NewErrVenueNotFound(venueId uuid.UUID) *CustomError {
	return NotFound(fmt.Sprintf(ErrVenueNotFound, venueId.String()))
}
