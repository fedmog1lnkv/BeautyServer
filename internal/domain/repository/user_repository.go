package repository

import (
	"beauty-server/internal/domain/entity"
	"beauty-server/internal/domain/value_object"
	"github.com/google/uuid"
)

type UserRepository interface {
	GetById(id uuid.UUID) (*entity.User, error)
	Save(user *entity.User) error
	Update(user *entity.User) error
	GetByPhoneNumber(phoneNumber value_object.UserPhoneNumber) (*entity.User, error)
	IsNameUnique(name value_object.UserName) (bool, error)
	IsPhoneNumberUnique(phoneNumber value_object.UserPhoneNumber) (bool, error)
}
