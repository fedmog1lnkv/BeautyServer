package router

import (
	"beauty-server/internal/api/handler/staff"
	"github.com/labstack/echo/v4"
)

func RegisterStaffRoutes(e *echo.Echo, staffHandler *staff.StaffHandler) {
	routes := e.Group("/staff")
	routes.POST("/phone_challenge", staffHandler.PhoneChallenge)
	routes.POST("/auth", staffHandler.Auth)
	//routes.POST("/refresh_token", userHandler.RefreshToken)
}
