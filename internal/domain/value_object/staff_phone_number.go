package value_object

import (
	"beauty-server/internal/domain/errors"
	"regexp"
	"strings"
)

const (
	StaffPhoneNumberRegex = `^\+?[1-9]\d{1,14}$`
)

type StaffPhoneNumber struct {
	value string
}

func NewStaffPhoneNumber(number string) (StaffPhoneNumber, error) {
	number = strings.TrimSpace(number)

	re := regexp.MustCompile(`\D`)
	number = re.ReplaceAllString(number, "")

	if strings.HasPrefix(number, "8") {
		number = "7" + number[1:]
	}

	match, _ := regexp.MatchString(UserPhoneNumberRegex, number)
	if !match {
		return StaffPhoneNumber{}, errors.NewErrStaffPhoneNumberInvalidFormat()
	}

	return StaffPhoneNumber{value: number}, nil
}

func (p StaffPhoneNumber) Value() string {
	return p.value
}

func (p StaffPhoneNumber) Equal(other StaffPhoneNumber) bool {
	return p.value == other.value
}
