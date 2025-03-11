package staff

import (
	"beauty-server/internal/api/common"
	"github.com/labstack/echo/v4"
	"net/http"
)

// AuthRequest represents the request body for user authentication.
// swagger:parameters auth
type AuthRequest struct {
	PhoneNumber string `json:"phone_number" example:"+1234567890"`
	Code        string `json:"code" example:"312-547"`
}

// AuthResponse represents the response body for a successful authentication.
// swagger:response authResponse
type AuthResponse struct {
	Token        string `json:"token" example:"jwt-token-example"`
	RefreshToken string `json:"refresh_token"  example:"refresh-token-example"`
}

// Auth allows the staff to authenticate using their phone number and verification code.
// @Summary Authenticate staff
// @Description This endpoint authenticates a staff by verifying their phone number and code.
// @Tags staff
// @Accept json
// @Produce json
// @Param request body AuthRequest true "Authentication request parameters"
// @Success 200 {object} AuthResponse "Successful authentication"
// @Router /staff/auth [post]
func (h *StaffHandler) Auth(c echo.Context) error {
	var request AuthRequest

	if err := c.Bind(&request); err != nil {
		return c.JSON(http.StatusBadRequest, map[string]string{"error": "Invalid request body"})
	}

	token, refreshToken, err := h.staffService.Auth(request.PhoneNumber, request.Code)
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
