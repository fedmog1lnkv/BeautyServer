package entity

import (
	"beauty-server/internal/domain/entity"
	"gorm.io/gorm"
)

type OrganizationConfig struct{}

func (OrganizationConfig) Apply(db *gorm.DB) error {
	return db.AutoMigrate(&entity.Organization{})
}
