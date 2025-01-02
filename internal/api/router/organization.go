package router

import (
	"beauty-server/internal/api/handler/organization"
	"github.com/labstack/echo/v4"
)

func RegisterOrganizationRoutes(e *echo.Echo, organizationHandler *organization.OrganizationHandler) {
	userRoutes := e.Group("/organization")
	userRoutes.POST("/", organizationHandler.Create)
	userRoutes.GET("/", organizationHandler.GetAll)
	userRoutes.GET("/:id", organizationHandler.GetById)
	userRoutes.PATCH("/", organizationHandler.Update)
}
