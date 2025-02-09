package value_object

import (
	"beauty-server/internal/domain/errors"
	"fmt"
)

type Location struct {
	Latitude  float64
	Longitude float64
}

func NewLocation(latitude, longitude float64) (Location, error) {
	if latitude < -90 || latitude > 90 {
		return Location{}, errors.NewErrLatitudeOutOfRange(latitude)
	}
	if longitude < -180 || longitude > 180 {
		return Location{}, errors.NewErrLongitudeOutOfRange(longitude)
	}
	return Location{Latitude: latitude, Longitude: longitude}, nil
}

func (loc Location) Equal(other Location) bool {
	return loc.Latitude == other.Latitude && loc.Longitude == other.Longitude
}

func (loc Location) String() string {
	return fmt.Sprintf("Latitude: %.6f, Longitude: %.6f", loc.Latitude, loc.Longitude)
}
