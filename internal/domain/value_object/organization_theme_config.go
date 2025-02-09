package value_object

import (
	"beauty-server/internal/domain/errors"
	"strings"
)

type OrganizationThemeConfig struct {
	color string
	photo *string
}

func NewOrganizationThemeConfig(color string, photo *string) (OrganizationThemeConfig, error) {
	color = strings.ToUpper(strings.TrimSpace(color))

	if len(color) != 7 || color[0] != '#' || !isValidHex(color[1:]) {
		return OrganizationThemeConfig{}, errors.NewErrOrganizationColorInvalidFormat()
	}

	return OrganizationThemeConfig{
		color: color,
		photo: photo,
	}, nil
}

func isValidHex(s string) bool {
	for _, c := range s {
		if !((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F')) {
			return false
		}
	}
	return true
}

func (c OrganizationThemeConfig) GetColor() string {
	return c.color
}

func (c OrganizationThemeConfig) GetPhoto() *string {
	return c.photo
}

func (c OrganizationThemeConfig) Value() (string, *string) {
	return c.color, c.photo
}

func (c OrganizationThemeConfig) Equal(other OrganizationThemeConfig) bool {
	if c.photo == nil && other.photo == nil {
		return c.color == other.color
	}
	if c.photo != nil && other.photo != nil {
		return c.color == other.color && *c.photo == *other.photo
	}
	return false
}
