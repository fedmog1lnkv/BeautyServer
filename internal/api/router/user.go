package router

import (
	"beauty-server/internal/api/handler/user"
	"github.com/labstack/echo/v4"
)

func RegisterUserRoutes(e *echo.Echo, userHandler *user.UserHandler) {
	userRoutes := e.Group("/user")
	userRoutes.POST("/phone_challenge", userHandler.PhoneChallenge)
	userRoutes.POST("/auth", userHandler.Auth)
	userRoutes.POST("/refresh_token", userHandler.RefreshToken)
}
