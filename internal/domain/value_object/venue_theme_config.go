package value_object

import (
	"beauty-server/internal/domain/errors"
	"strings"
)

type VenueThemeConfig struct {
	color string
	photo *string
}

func NewVenueThemeConfig(color string, photo *string) (VenueThemeConfig, error) {
	color = strings.ToUpper(strings.TrimSpace(color))

	if len(color) != 7 || color[0] != '#' || !isValidHex(color[1:]) {
		return VenueThemeConfig{}, errors.NewErrVenueColorInvalidFormat()
	}

	return VenueThemeConfig{
		color: color,
		photo: photo,
	}, nil
}

func (c VenueThemeConfig) GetColor() string {
	return c.color
}

func (c VenueThemeConfig) GetPhoto() *string {
	return c.photo
}

func (c VenueThemeConfig) Value() (string, *string) {
	return c.color, c.photo
}

func (c VenueThemeConfig) Equal(other VenueThemeConfig) bool {
	if c.photo == nil && other.photo == nil {
		return c.color == other.color
	}
	if c.photo != nil && other.photo != nil {
		return c.color == other.color && *c.photo == *other.photo
	}
	return false
}
