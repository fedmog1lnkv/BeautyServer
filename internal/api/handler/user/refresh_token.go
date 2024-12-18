package user

import (
	"github.com/labstack/echo/v4"
	"net/http"
)

// RefreshTokenRequest represents the request body for refreshing a user's token.
// swagger:parameters refreshToken
type RefreshTokenRequest struct {
	RefreshToken string `json:"refresh_token" example:"refresh-token-example"`
}

// RefreshTokenResponse represents the response body for a successful token refresh.
// swagger:response refreshTokenResponse
type RefreshTokenResponse struct {
	Token        string `json:"token" example:"new-jwt-token-example"`
	RefreshToken string `json:"refresh_token" example:"new-refresh-token-example"`
}

// RefreshToken allows the user to refresh their token using the provided refresh token.
// @Summary refresh token
// @Description This endpoint allows the user to refresh their JWT token by providing a valid refresh token.
// @Tags user
// @Accept json
// @Produce json
// @Param request body RefreshTokenRequest true "Refresh token parameters"
// @Success 200 {object} RefreshTokenResponse "Successful token refresh"
// @Router /user/refresh-token [post]
func (h *UserHandler) RefreshToken(c echo.Context) error {
	var request RefreshTokenRequest

	if err := c.Bind(&request); err != nil {
		return c.JSON(http.StatusBadRequest, map[string]string{"error": "Invalid request body"})
	}

	token, refreshToken, err := h.userService.RefreshToken(request.RefreshToken)
	if err != nil {
		return c.JSON(http.StatusInternalServerError, map[string]string{"error": "Failed to generate token"})
	}

	response := RefreshTokenResponse{
		Token:        token,
		RefreshToken: refreshToken,
	}

	return c.JSON(http.StatusOK, response)
}
