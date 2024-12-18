package value_object

import (
	"beauty-server/internal/domain/errors"
	"regexp"
	"strings"
)

const (
	UserPhoneNumberRegex = `^\+?[1-9]\d{1,14}$`
)

type UserPhoneNumber struct {
	value string
}

func NewUserPhoneNumber(number string) (UserPhoneNumber, error) {
	number = strings.TrimSpace(number)

	match, err := regexp.MatchString(UserPhoneNumberRegex, number)
	if err != nil {
		return UserPhoneNumber{}, err
	}
	if !match {
		return UserPhoneNumber{}, errors.NewErrUserPhoneNumberInvalidFormat()
	}

	return UserPhoneNumber{value: number}, nil
}

func (p UserPhoneNumber) Value() string {
	return p.value
}

func (p UserPhoneNumber) Equal(other UserPhoneNumber) bool {
	return p.value == other.value
}
