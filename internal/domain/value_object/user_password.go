package value_object

import (
	"beauty-server/internal/domain/errors"
	"strings"
)

const (
	MinPasswordLength = 6
)

type UserPassword struct {
	value string
}

func NewUserPassword(password string) (UserPassword, error) {
	password = strings.TrimSpace(password)

	if len(password) < MinPasswordLength {
		return UserPassword{}, errors.NewErrUserPasswordTooShort(MinPasswordLength)
	}

	return UserPassword{value: string(password)}, nil
}

func (p UserPassword) Value() string {
	return p.value
}

func (p UserPassword) Equal(other UserPassword) bool {
	return p.value == other.value
}
