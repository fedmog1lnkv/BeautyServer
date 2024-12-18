package di

import (
	"beauty-server/internal/application/service/user"
	"go.uber.org/fx"
)

var ServiceContainer = fx.Provide(
	user.NewUserService,
)
