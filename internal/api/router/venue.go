package router

import (
	"beauty-server/internal/api/handler/venue"
	"github.com/labstack/echo/v4"
)

func RegisterVenueRoutes(e *echo.Echo, venueHandler *venue.VenueHandler) {
	venueRoutes := e.Group("/venue")
	venueRoutes.POST("/", venueHandler.Create)
	/*	venueRoutes.GET("/", venueHandler.GetAll)
		venueRoutes.GET("/:id", venueHandler.GetById)
		venueRoutes.GET("/:id", venueHandler.GetByLocation)
		venueRoutes.PATCH("/", venueHandler.Update)*/
}
