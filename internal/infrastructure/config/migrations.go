package config

import (
	"beauty-server/internal/infrastructure/model"
	"fmt"
	"gorm.io/gorm"
	"log"
)

func MigrateEntities(db *gorm.DB) error {
	if err := InitializaeEnumsInDb(db); err != nil {
		return err
	}

	err := db.AutoMigrate(
		&model.UserModel{},
		&model.OrganizationModel{},
		&model.VenueModel{},
		&model.PhoneChallengeModel{},
		&model.ServiceModel{},
		&model.TimeSlotModel{},
		&model.StaffModel{},
	)

	if err != nil {
		return err
	}

	log.Println("Entities migrated successfully.")
	return nil
}

func InitializaeEnumsInDb(db *gorm.DB) error {
	err := db.Exec(`
		DO $do$
		BEGIN
			IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'organization_subscription') THEN
				CREATE TYPE organization_subscription AS ENUM ('Active', 'Disabled');
			END IF;
		END $do$
	`).Error
	if err != nil {
		return fmt.Errorf("failed to create ENUM type organization_subscription: %v", err)
	}

	err = db.Exec(`
		DO $do$
		BEGIN
			IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'staff_role') THEN
				CREATE TYPE staff_role AS ENUM ('Manager', 'Master', 'Unknown');
			END IF;
		END $do$
	`).Error
	if err != nil {
		return fmt.Errorf("failed to create ENUM type staff_role: %v", err)
	}

	log.Println("ENUM types initialized successfully.")
	return nil
}
