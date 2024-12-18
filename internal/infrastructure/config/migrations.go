package config

import (
	"beauty-server/internal/infrastructure/model"
	"gorm.io/gorm"
	"log"
)

func MigrateEntities(db *gorm.DB) error {
	err := db.AutoMigrate(&model.UserModel{})
	if err != nil {
		return err
	}

	log.Println("Entities migrated successfully.")
	return nil
}
