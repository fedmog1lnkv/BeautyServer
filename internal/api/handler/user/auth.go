package user

import (
	"beauty-server/internal/api/common"
	"github.com/labstack/echo/v4"
	"net/http"
)

type AuthRequest struct {
	PhoneNumber string `json:"phone_number" example:"+1234567890"`
	Code        string `json:"code" example:"312-547"`
}

type AuthResponse struct {
	Token        string `json:"token" example:"jwt-token-example"`
	RefreshToken string `json:"refresh_token"  example:"refresh-token-example"`
}

func (h *UserHandler) Auth(c echo.Context) error {
	var request AuthRequest

	if err := c.Bind(&request); err != nil {
		return c.JSON(http.StatusBadRequest, map[string]string{"error": "Invalid request body"})
	}

	token, refreshToken, err := h.userService.Auth(request.PhoneNumber, request.Code)
	if err != nil {
		common.HandleFailure(err, c)
		return nil
	}

	response := AuthResponse{
		Token:        token,
		RefreshToken: refreshToken,
	}

	return c.JSON(http.StatusOK, response)
}
