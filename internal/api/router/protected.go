package router

import (
	"beauty-server/internal/api/handler/protected"
	"beauty-server/internal/api/middleware"
	"github.com/labstack/echo/v4"
)

func RegisterProtectedRoutes(e *echo.Echo, protectedHandler *protected.ProtectedHandler) {
	protectedRoutes := e.Group("/protected")
	protectedRoutes.Use(middleware.JWTMiddleware)
	protectedRoutes.POST("", protectedHandler.ProtectedRoute)
}
