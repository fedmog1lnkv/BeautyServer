package entity

import (
	"beauty-server/internal/domain/entity"
	"gorm.io/gorm"
)

type VenueConfig struct{}

func (VenueConfig) Apply(db *gorm.DB) error {
	return db.AutoMigrate(&entity.Venue{})
}
