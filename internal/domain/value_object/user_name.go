package value_object

import (
	"beauty-server/internal/domain/errors"
	"strings"
)

const (
	UserNameMinLength = 3
	UserNameMaxLength = 50
)

type UserName struct {
	value string
}

func NewUserName(name string) (UserName, error) {
	name = strings.TrimSpace(name)
	if len(name) < UserNameMinLength {
		return UserName{}, errors.NewErrUserNameTooShort(UserNameMinLength)
	}
	if len(name) > UserNameMaxLength {
		return UserName{}, errors.NewErrUserNameTooLong(UserNameMaxLength)
	}
	return UserName{value: name}, nil
}

func (u UserName) Value() string {
	return u.value
}

func (u UserName) Equal(other UserName) bool {
	return u.value == other.value
}
