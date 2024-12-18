package user

import (
	"fmt"
	"github.com/labstack/echo/v4"
	"net/http"
)

// LoginUserRequest represents the request body for user login.
// swagger:parameters loginUser
type LoginUserRequest struct {
	PhoneNumber string `json:"phone_number" example:"+1234567890"`
	Password    string `json:"password" example:"password123"`
}

// LoginUserResponse represents the response body for user login.
// swagger:response loginUserResponse
type LoginUserResponse struct {
	Token        string `json:"token" example:"jwt-token-example"`
	RefreshToken string `json:"refresh_token"  example:"refresh-token-example"`
}

// LoginUser allows a user to log in with their phone number and password.
// @Summary login
// @Description This endpoint allows a user to log in by providing their phone number and password.
// @Tags user
// @Accept json
// @Produce json
// @Param request body LoginUserRequest true "Login parameters"
// @Success 200 {object} LoginUserResponse "Successful login"
// @Router /user/login [post]
func (h *UserHandler) LoginUser(c echo.Context) error {
	var request LoginUserRequest

	if err := c.Bind(&request); err != nil {
		return c.JSON(http.StatusBadRequest, map[string]string{"error": "Invalid request body"})
	}

	token, refreshToken, err := h.userService.Login(request.PhoneNumber, request.Password)
	if err != nil {
		return c.JSON(http.StatusUnauthorized, map[string]string{"error": fmt.Sprintf("authentication failed: %v", err)})
	}

	response := LoginUserResponse{
		Token:        token,
		RefreshToken: refreshToken,
	}

	return c.JSON(http.StatusOK, response)
}
