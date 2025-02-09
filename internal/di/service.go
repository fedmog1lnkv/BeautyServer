package di

import (
	"beauty-server/internal/application/service/organization"
	"beauty-server/internal/application/service/user"
	"beauty-server/internal/application/service/venue"
	"go.uber.org/fx"
)

var ServiceContainer = fx.Provide(
	user.NewUserService,
	organization.NewOrganizationService,
	venue.NewVenueService,
)
