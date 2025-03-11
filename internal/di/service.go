package di

import (
	"beauty-server/internal/application/service/organization"
	"beauty-server/internal/application/service/service"
	"beauty-server/internal/application/service/staff"
	"beauty-server/internal/application/service/user"
	"beauty-server/internal/application/service/venue"
	"go.uber.org/fx"
)

var ServiceContainer = fx.Provide(
	user.NewUserService,
	staff.NewStaffService,
	organization.NewOrganizationService,
	venue.NewVenueService,
	service.NewServiceService,
)
