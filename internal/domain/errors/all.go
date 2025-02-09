package errors

import "fmt"

const (
	ErrLatitudeOutOfRange  = "latitude must be between -90 and 90, but got %.2f"
	ErrLongitudeOutOfRange = "longitude must be between -180 and 180, but got %.2f"
)

func NewErrLatitudeOutOfRange(latitude float64) *CustomError {
	return Validation(fmt.Sprintf(ErrLatitudeOutOfRange, latitude))
}

func NewErrLongitudeOutOfRange(longitude float64) *CustomError {
	return Validation(fmt.Sprintf(ErrLongitudeOutOfRange, longitude))
}
