package di

import (
	"beauty-server/internal/api/handler/organization"
	"beauty-server/internal/api/handler/protected"
	"beauty-server/internal/api/handler/user"
	"go.uber.org/fx"
)

var (
	HandlerContainer = fx.Provide(
		user.NewUserHandler,
		organization.NewOrganizationHandler,
		protected.NewProtectedHandler,
	)
)
