package middleware

import (
	"beauty-server/internal/infrastructure/auth"
	"github.com/labstack/echo/v4"
	"log"
	"net/http"
	"strings"
)

func UserMiddleware(next echo.HandlerFunc) echo.HandlerFunc {
	return func(c echo.Context) error {
		tokenString := c.Request().Header.Get("Authorization")
		if tokenString == "" {
			return c.JSON(http.StatusUnauthorized, map[string]string{"error": "Missing Authorization header"})
		}

		parts := strings.Split(tokenString, " ")
		if len(parts) != 2 || parts[0] != "Bearer" {
			return c.JSON(http.StatusUnauthorized, map[string]string{"error": "Invalid Authorization header"})
		}

		token := parts[1]

		claims, err := auth.ParseToken(token)
		if err != nil {
			log.Println("Failed to parse token:", err)
			return c.JSON(http.StatusUnauthorized, map[string]string{"error": "Invalid or expired token, you need to use refresh token or authorize again"})
		}

		// Changing the isAdmin so that the admin can perform requests on behalf of a regular user
		isAdminHeader := c.Request().Header.Get("IsAdmin")
		if isAdminHeader != "true" {
			claims.IsAdmin = false
		}

		c.Set("user_id", claims.UserID)
		c.Set("is_admin", claims.IsAdmin)

		return next(c)
	}
}
