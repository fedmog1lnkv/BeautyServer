package router

import (
	"beauty-server/internal/api/handler/user"
	"github.com/labstack/echo/v4"
)

func RegisterUserRoutes(e *echo.Echo, userHandler *user.UserHandler) {
	userRoutes := e.Group("/users")
	userRoutes.POST("/register", userHandler.RegisterUser)
	userRoutes.GET("/login", userHandler.LoginUser)
	userRoutes.POST("/refresh_token", userHandler.RefreshToken)
}
