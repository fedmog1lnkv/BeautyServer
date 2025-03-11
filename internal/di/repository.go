package di

import (
	"beauty-server/internal/infrastructure/repository"
	"go.uber.org/fx"
)

var RepositoryContainer = fx.Provide(
	repository.NewUserRepository,
	repository.NewStaffRepository,
	repository.NewOrganizationRepository,
	repository.NewVenueRepository,
	repository.NewPhoneChallengeRepository,
	repository.NewServiceRepository,
)
