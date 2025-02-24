package di

import (
	"beauty-server/internal/api/handler/organization"
	"beauty-server/internal/api/handler/protected"
	"beauty-server/internal/api/handler/service"
	"beauty-server/internal/api/handler/user"
	"beauty-server/internal/api/handler/venue"
	"go.uber.org/fx"
)

var (
	HandlerContainer = fx.Provide(
		user.NewUserHandler,
		organization.NewOrganizationHandler,
		venue.NewVenueHandler,
		service.NewServiceHandler,
		protected.NewProtectedHandler,
	)
)
