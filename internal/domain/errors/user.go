package errors

import "fmt"

const (
	ErrUserNameTooShort           = "user name must be at least %d characters long"
	ErrUserNameTooLong            = "user name must not exceed %d characters"
	ErrUserPhoneNumberInvalid     = "invalid user phone number format"
	ErrUserNotFound               = "user not found"
	ErrUserNameIsNotUnique        = "user with name '%s' already exists"
	ErrUserPhoneNumberIsNotUnique = "user with same phone number '%s' already exists"
	ErrUserPasswordTooShort       = "user password must be at least %d characters long"
)

func NewErrUserNameTooShort(minLength int) *CustomError {
	return Validation(fmt.Sprintf(ErrUserNameTooShort, minLength))
}

func NewErrUserNameTooLong(maxLength int) *CustomError {
	return Validation(fmt.Sprintf(ErrUserNameTooLong, maxLength))
}

func NewErrUserPhoneNumberInvalidFormat() *CustomError {
	return Validation(ErrUserPhoneNumberInvalid)
}

func NewErrUserNotFound() *CustomError {
	return NotFound(ErrUserNotFound)
}

func NewErrUserNameIsNotUnique(name string) *CustomError {
	return Conflict(fmt.Sprintf(ErrUserNameIsNotUnique, name))
}

func NewErrUserPhoneNumberIsNotUnique(phoneNumber string) *CustomError {
	return Conflict(fmt.Sprintf(ErrUserPhoneNumberIsNotUnique, phoneNumber))
}

func NewErrUserPasswordTooShort(minLength int) *CustomError {
	return Validation(fmt.Sprintf(ErrUserPasswordTooShort, minLength))
}
