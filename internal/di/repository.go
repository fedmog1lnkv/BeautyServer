package di

import (
	"beauty-server/internal/infrastructure/repository"
	"go.uber.org/fx"
)

var RepositoryContainer = fx.Provide(
	repository.NewUserRepository,
)
