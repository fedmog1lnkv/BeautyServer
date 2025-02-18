package entity

import (
	"beauty-server/internal/domain/entity"
	"gorm.io/gorm"
)

type PhoneChallengeConfig struct{}

func (PhoneChallengeConfig) Apply(db *gorm.DB) error {
	return db.AutoMigrate(&entity.PhoneChallenge{})
}
