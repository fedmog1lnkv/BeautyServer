package entity

import (
	"beauty-server/internal/domain/entity"
	"gorm.io/gorm"
)

type ServiceConfig struct{}

func (ServiceConfig) Apply(db *gorm.DB) error {
	return db.AutoMigrate(&entity.Service{})
}
