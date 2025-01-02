package repository

import (
	"beauty-server/internal/domain/entity"
	"beauty-server/internal/domain/repository"
	"beauty-server/internal/domain/value_object"
	"beauty-server/internal/infrastructure/model"
	"fmt"
	"github.com/google/uuid"
	"gorm.io/gorm"
)

type UserRepository struct {
	DB *gorm.DB
}

func NewUserRepository(db *gorm.DB) repository.UserRepository {
	return &UserRepository{DB: db}
}

func (r *UserRepository) GetById(id uuid.UUID) (*entity.User, error) {
	var userModel model.UserModel
	err := r.DB.Where("id = ?", id).First(&userModel).Error
	if err != nil {
		if err == gorm.ErrRecordNotFound {
			return nil, nil
		}
		return nil, fmt.Errorf("error fetching user by id: %v", err)
	}

	user, err := userModel.ToDomain()
	if err != nil {
		return nil, err
	}

	return user, nil
}

func (r *UserRepository) Save(user *entity.User) error {
	userModel := model.FromDomainUser(user)
	if err := r.DB.Create(userModel).Error; err != nil {
		return fmt.Errorf("failed to save user: %v", err)
	}
	return nil
}

func (r *UserRepository) Update(user *entity.User) error {
	userModel := model.FromDomainUser(user)
	if err := r.DB.Save(userModel).Error; err != nil {
		return fmt.Errorf("failed to update user: %v", err)
	}
	return nil
}

func (r *UserRepository) IsNameUnique(name value_object.UserName) (bool, error) {
	var user model.UserModel
	err := r.DB.Where("name = ?", name.Value()).First(&user).Error
	if err != nil {
		if err == gorm.ErrRecordNotFound {
			return true, nil
		}
		return false, err
	}
	return false, nil
}

func (r *UserRepository) GetByPhoneNumber(phoneNumber value_object.UserPhoneNumber) (*entity.User, error) {
	var userModel model.UserModel
	err := r.DB.Where("phone_number = ?", phoneNumber.Value()).First(&userModel).Error
	if err != nil {
		if err == gorm.ErrRecordNotFound {
			return nil, nil
		}
		return nil, fmt.Errorf("error fetching user by phone number: %v", err)
	}

	user, err := userModel.ToDomain()
	if err != nil {
		return nil, err
	}

	return user, nil
}

func (r *UserRepository) IsPhoneNumberUnique(phoneNumber value_object.UserPhoneNumber) (bool, error) {
	var user model.UserModel
	err := r.DB.Where("phone_number = ?", phoneNumber.Value()).First(&user).Error
	if err != nil {
		if err == gorm.ErrRecordNotFound {
			return true, nil
		}
		return false, err
	}
	return false, nil
}
