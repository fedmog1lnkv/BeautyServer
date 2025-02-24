package router

import (
	"beauty-server/internal/api/handler/service"
	"github.com/labstack/echo/v4"
)

func RegisterServiceRoutes(e *echo.Echo, handler *service.ServiceHandler) {
	routes := e.Group("/service")
	routes.POST("", handler.Create)
}
