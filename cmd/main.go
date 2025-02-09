// @title BeautyServerAPI
// @description This is a server for automating appointments to beauty salons
// @version 1.0

package main

import (
	"beauty-server/internal/api/handler/organization"
	"beauty-server/internal/api/handler/protected"
	"beauty-server/internal/api/handler/user"
	"beauty-server/internal/api/handler/venue"
	"beauty-server/internal/api/router"
	"beauty-server/internal/di"
	"beauty-server/internal/infrastructure/config"
	"github.com/joho/godotenv"
	"github.com/labstack/echo/v4"
	"github.com/labstack/echo/v4/middleware"
	"go.uber.org/fx"
	"log"
	"os"
)

func main() {
	if err := godotenv.Load(); err != nil {
		log.Println("No .env file found, using environment variables")
	}

	app := fx.New(
		di.AppContainer,
		fx.Invoke(config.MigrateEntities),
		fx.Invoke(registerRouters),
		fx.Invoke(runServer),
	)

	app.Run()
}

func registerRouters(
	e *echo.Echo,
	userHandler *user.UserHandler,
	protectedHandler *protected.ProtectedHandler,
	organizationHandler *organization.OrganizationHandler,
	venueHandler *venue.VenueHandler,
) {
	// TODO: move to different func
	e.Use(middleware.CORSWithConfig(middleware.CORSConfig{
		AllowOrigins: []string{"*"},
		AllowMethods: []string{echo.GET, echo.POST, echo.PUT, echo.DELETE},
		AllowHeaders: []string{echo.HeaderContentType, echo.HeaderAuthorization},
	}))

	router.RegisterUserRoutes(e, userHandler)
	router.RegisterProtectedRoutes(e, protectedHandler)
	router.RegisterOrganizationRoutes(e, organizationHandler)
	router.RegisterVenueRoutes(e, venueHandler)
}

func runServer(e *echo.Echo) {
	port := os.Getenv("PORT")
	if port == "" {
		port = "8080"
	}

	log.Printf("Starting server on port %s", port)

	e.Static("/swagger", "/docs")

	e.Logger.Fatal(e.Start(":" + port))
}
