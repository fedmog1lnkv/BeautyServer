package router

import (
	"beauty-server/internal/api/handler/organization"
	"beauty-server/internal/api/middleware"
	"github.com/labstack/echo/v4"
)

func RegisterOrganizationRoutes(e *echo.Echo, organizationHandler *organization.OrganizationHandler) {
	orgRoutes := e.Group("/organization")
	orgRoutes.POST("", organizationHandler.Create, middleware.AdminMiddleware)
	orgRoutes.GET("", organizationHandler.GetAll, middleware.UserMiddleware)
	orgRoutes.GET("/:id", organizationHandler.GetById, middleware.UserMiddleware)
	// TODO : add staffMiddleware
	orgRoutes.PATCH("", organizationHandler.Update, middleware.UserMiddleware)
}
