package value_object

import (
	"strings"
)

type OrganizationPhoto struct {
	value string
}

func NewOrganizationPhoto(url string) (OrganizationPhoto, error) {
	url = strings.TrimSpace(url)
	return OrganizationPhoto{value: url}, nil
}

func (p OrganizationPhoto) Value() string {
	return p.value
}

func (p OrganizationPhoto) Equal(other OrganizationPhoto) bool {
	return p.value == other.value
}
