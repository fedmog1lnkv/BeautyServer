package enum

import (
	"database/sql/driver"
	"fmt"
	"strings"
)

type OrganizationSubscription int

const (
	Active   OrganizationSubscription = iota // Активная подписка
	Disabled                                 // Отключённая подписка
)

func (s OrganizationSubscription) String() string {
	switch s {
	case Active:
		return "Active"
	case Disabled:
		return "Disabled"
	default:
		return "Unknown"
	}
}

func (s *OrganizationSubscription) Scan(value interface{}) error {
	if v, ok := value.(string); ok {
		switch v {
		case "Active":
			*s = Active
		case "Disabled":
			*s = Disabled
		default:
			return fmt.Errorf("failed to scan OrganizationSubscription, unknown value %s", v)
		}
		return nil
	}
	return fmt.Errorf("failed to scan OrganizationSubscription, unsupported type %T", value)
}

func ParseOrganizationSubscription(value string) (OrganizationSubscription, error) {
	switch strings.ToLower(value) {
	case "active":
		return Active, nil
	case "disabled":
		return Disabled, nil
	default:
		return 0, fmt.Errorf("invalid subscription value: %s", value)
	}
}

func (s OrganizationSubscription) Value() (driver.Value, error) {
	return s.String(), nil
}
