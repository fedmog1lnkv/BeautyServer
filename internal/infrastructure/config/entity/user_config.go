package entity

import (
	"beauty-server/internal/domain/entity"
	"gorm.io/gorm"
)

type UserConfig struct{}

func (UserConfig) Apply(db *gorm.DB) error {
	return db.AutoMigrate(&entity.User{})
}
