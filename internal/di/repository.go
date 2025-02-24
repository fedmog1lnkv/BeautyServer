package di

import (
	"beauty-server/internal/infrastructure/repository"
	"go.uber.org/fx"
)

var RepositoryContainer = fx.Provide(
	repository.NewUserRepository,
	repository.NewOrganizationRepository,
	repository.NewVenueRepository,
	repository.NewPhoneChallengeRepository,
	repository.NewServiceRepository,
)
