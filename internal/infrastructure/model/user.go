package model

import (
	"beauty-server/internal/domain/entity"
	"beauty-server/internal/domain/value_object"
	"github.com/google/uuid"
)

type UserModel struct {
	ID          uuid.UUID `gorm:"type:uuid;primaryKey"`
	Name        string    `gorm:"type:varchar(255);not null"`
	PhoneNumber string    `gorm:"type:varchar(20);not null;unique"`
	Password    string    `gorm:"type:varchar(255);not null"`
}

func (UserModel) TableName() string {
	return "users"
}

func (m *UserModel) ToDomain() (*entity.User, error) {
	name, err := value_object.NewUserName(m.Name)
	if err != nil {
		return nil, err
	}

	phoneNumber, err := value_object.NewUserPhoneNumber(m.PhoneNumber)
	if err != nil {
		return nil, err
	}

	password, err := value_object.NewUserPassword(m.Password)
	if err != nil {
		return nil, err
	}

	return &entity.User{
		Id:          m.ID,
		Name:        name,
		PhoneNumber: phoneNumber,
		Password:    password,
	}, nil
}

func FromDomain(user *entity.User) *UserModel {
	return &UserModel{
		ID:          user.Id,
		Name:        user.Name.Value(),
		PhoneNumber: user.PhoneNumber.Value(),
		Password:    user.Password.Value(),
	}
}
