package value_object

import (
	"beauty-server/internal/domain/errors"
	"strings"
)

const (
	StaffNameMinLength = 3
	StaffNameMaxLength = 50
)

type StaffName struct {
	value string
}

func NewStaffName(name string) (StaffName, error) {
	name = strings.TrimSpace(name)
	if len(name) < StaffNameMinLength {
		return StaffName{}, errors.NewErrStaffNameTooShort(StaffNameMinLength)
	}
	if len(name) > StaffNameMaxLength {
		return StaffName{}, errors.NewErrStaffNameTooLong(StaffNameMaxLength)
	}
	return StaffName{value: name}, nil
}

func (u StaffName) Value() string {
	return u.value
}

func (u StaffName) Equal(other StaffName) bool {
	return u.value == other.value
}
