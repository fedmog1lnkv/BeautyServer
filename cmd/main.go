// @title BeautyServerAPI
// @description This is a server for automating appointments to beauty salons
// @version 1.0

package main

import (
	"beauty-server/internal/api/handler/protected"
	"beauty-server/internal/api/handler/user"
	"beauty-server/internal/api/router"
	"beauty-server/internal/di"
	"github.com/joho/godotenv"
	"github.com/labstack/echo/v4"
	"go.uber.org/fx"
	"log"
	"os"
	"os/exec"
)

func main() {
	if err := godotenv.Load(); err != nil {
		log.Fatalf("Error loading .env file")
	}

	app := fx.New(
		di.AppContainer,
		fx.Invoke(registerRouters),
		fx.Invoke(generateDocs),
		fx.Invoke(runServer),
	)

	app.Run()
}

func registerRouters(
	e *echo.Echo,
	userHandler *user.UserHandler,
	protectedHandler *protected.ProtectedHandler,
) {
	router.RegisterUserRoutes(e, userHandler)
	router.RegisterProtectedRoutes(e, protectedHandler)
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

func generateDocs() {
	cmd := exec.Command("swag", "init", "-g", "cmd/main.go")
	cmd.Dir = "./"
	err := cmd.Run()
	if err != nil {
		log.Fatalf("Error while generating documentation: %v", err)
	}
	log.Println("Swagger documentation generated successfully!")
}
