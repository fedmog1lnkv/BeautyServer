package protected

import (
	"beauty-server/internal/application/service/user"
	"github.com/labstack/echo/v4"
	"net/http"
)

type ProtectedHandler struct {
}

func NewProtectedHandler(userService *user.UserService) *ProtectedHandler {
	return &ProtectedHandler{}
}

func (h *ProtectedHandler) ProtectedRoute(c echo.Context) error {
	userID := c.Get("user_id").(string)
	return c.JSON(http.StatusOK, map[string]string{"message": "Hello, " + userID})
}
