package config

import (
	"gorm.io/driver/postgres"
	"gorm.io/gorm"
	"log"
)

type DatabaseConfig struct {
	DSN string
}

func NewDatabaseConfig(dsn string) *DatabaseConfig {
	return &DatabaseConfig{DSN: dsn}
}

func (cfg *DatabaseConfig) Connect() (*gorm.DB, error) {
	db, err := gorm.Open(postgres.Open(cfg.DSN), &gorm.Config{})
	if err != nil {
		return nil, err
	}

	if err := MigrateEntities(db); err != nil {
		return nil, err
	}

	log.Println("Database connected and migrated.")
	return db, nil
}
