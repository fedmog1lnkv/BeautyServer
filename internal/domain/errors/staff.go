package errors

import (
	"fmt"
	"github.com/google/uuid"
)

const (
	ErrStaffNameTooShort           = "staff name must be at least %d characters long"
	ErrStaffNameTooLong            = "staff name must not exceed %d characters"
	ErrStaffPhoneNumberInvalid     = "invalid staff phone number format"
	ErrStaffNotFound               = "staff not found"
	ErrStaffPhoneNumberIsNotUnique = "staff with same phone number '%s' already exists"
	ErrStaffPasswordTooShort       = "staff password must be at least %d characters long"
	ErrTimeSlotNotFound            = "time slot not found with ID: %s"
)

func NewErrStaffNameTooShort(minLength int) *CustomError {
	return Validation(fmt.Sprintf(ErrStaffNameTooShort, minLength))
}

func NewErrStaffNameTooLong(maxLength int) *CustomError {
	return Validation(fmt.Sprintf(ErrStaffNameTooLong, maxLength))
}

func NewErrStaffPhoneNumberInvalidFormat() *CustomError {
	return Validation(ErrStaffPhoneNumberInvalid)
}

func NewErrStaffNotFound() *CustomError {
	return NotFound(ErrStaffNotFound)
}

func NewErrStaffPhoneNumberIsNotUnique(phoneNumber string) *CustomError {
	return Conflict(fmt.Sprintf(ErrStaffPhoneNumberIsNotUnique, phoneNumber))
}

func NewErrStaffPasswordTooShort(minLength int) *CustomError {
	return Validation(fmt.Sprintf(ErrStaffPasswordTooShort, minLength))
}

func NewErrTimeSlotNotFound(id uuid.UUID) *CustomError {
	return NotFound(fmt.Sprintf(ErrTimeSlotNotFound, id.String()))
}
