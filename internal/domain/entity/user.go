package entity

import (
	"beauty-server/internal/domain/errors"
	"beauty-server/internal/domain/value_object"
	"github.com/google/uuid"
)

type User struct {
	Id          uuid.UUID
	Name        value_object.UserName
	PhoneNumber value_object.UserPhoneNumber
}

type UserRepository interface {
	IsPhoneNumberUnique(phoneNumber value_object.UserPhoneNumber) (bool, error)
}

func NewUser(id uuid.UUID, name, phoneNumber string, userRepo UserRepository) (*User, error) {
	userName, err := value_object.NewUserName(name)
	if err != nil {
		return nil, err
	}

	userPhone, err := value_object.NewUserPhoneNumber(phoneNumber)
	if err != nil {
		return nil, err
	}
	isUniquePhone, err := userRepo.IsPhoneNumberUnique(userPhone)
	if err != nil {
		return nil, err
	}
	if !isUniquePhone {
		return nil, errors.NewErrUserPhoneNumberIsNotUnique(phoneNumber)
	}

	return &User{
		Id:          id,
		Name:        userName,
		PhoneNumber: userPhone,
	}, nil
}
