package router

import (
	"beauty-server/internal/api/handler/organization"
	"github.com/labstack/echo/v4"
)

func RegisterOrganizationRoutes(e *echo.Echo, organizationHandler *organization.OrganizationHandler) {
	orgRoutes := e.Group("/organization")
	orgRoutes.POST("/", organizationHandler.Create)
	orgRoutes.GET("/", organizationHandler.GetAll)
	orgRoutes.GET("/:id", organizationHandler.GetById)
	orgRoutes.PATCH("/", organizationHandler.Update)
}
