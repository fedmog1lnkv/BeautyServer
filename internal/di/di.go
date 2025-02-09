package di

import (
	"beauty-server/internal/domain/events"
	eventbusWrapper "beauty-server/internal/infrastructure/eventbus"
	"github.com/labstack/echo/v4"
	"go.uber.org/fx"
	"gorm.io/driver/postgres"
	"gorm.io/gorm"
	"log"
	"os"
)

var AppContainer = fx.Options(
	RepositoryContainer,
	ServiceContainer,
	HandlerContainer,
	EventBusContainer,
	fx.Provide(NewEcho),
	fx.Provide(NewGorm),
	fx.Provide(NewEventBus),
	fx.Invoke(RegisterEventHandlers),
)

func NewEcho() *echo.Echo {
	return echo.New()
}

func NewGorm() *gorm.DB {
	databaseDSN := os.Getenv("DATABASE_DSN")
	if databaseDSN == "" {
		log.Fatalf("Не указана строка подключения в переменной окружения DATABASE_DSN")
	}

	db, err := gorm.Open(postgres.Open(databaseDSN), &gorm.Config{})
	if err != nil {
		log.Fatalf("Ошибка подключения к базе данных: %v", err)
	}

	log.Print("Успешное подключение к бд")
	return db
}

func NewEventBus() events.EventBus {
	return eventbusWrapper.NewEventBusWrapper()
}
