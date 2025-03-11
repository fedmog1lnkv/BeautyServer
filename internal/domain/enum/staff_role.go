package enum

import (
	"database/sql/driver"
	"fmt"
	"strings"
)

type StaffRole int

const (
	Unknown = -1
	Manager = 0
	Master  = 1
)

func (s StaffRole) String() string {
	switch s {
	case Manager:
		return "Manager"
	case Master:
		return "Master"
	default:
		return "Unknown"
	}
}

func (s *StaffRole) Scan(value interface{}) error {
	if v, ok := value.(string); ok {
		switch v {
		case "Unknown":
			*s = Unknown
		case "Manager":
			*s = Manager
		case "Master":
			*s = Master
		default:
			return fmt.Errorf("failed to scan StaffRole, unknown value %s", v)
		}
		return nil
	}
	return fmt.Errorf("failed to scan StaffRole, unsupported type %T", value)
}

func ParseStaffRole(value string) (StaffRole, error) {
	switch strings.ToLower(value) {
	case "unknown":
		return Unknown, nil
	case "manager":
		return Manager, nil
	case "master":
		return Master, nil
	default:
		return 0, fmt.Errorf("invalid role value: %s", value)
	}
}

func (s StaffRole) Value() (driver.Value, error) {
	return s.String(), nil
}
