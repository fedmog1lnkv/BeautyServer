package user

import (
	"beauty-server/internal/api/common"
	"beauty-server/internal/domain/errors"
	"github.com/labstack/echo/v4"
	"net/http"
)

// RegisterUserRequest represents the request body for user registration.
// swagger:parameters registerUser
type RegisterUserRequest struct {
	Name        string `json:"name" example:"John Doe"`
	PhoneNumber string `json:"phone_number" example:"+1234567890"`
	Password    string `json:"password" example:"password123"`
}

// RegisterUserResponse represents the response body for user registration.
// swagger:response registerUserResponse
type RegisterUserResponse struct {
	Token        string `json:"token" example:"jwt-token-example"`
	RefreshToken string `json:"refresh_token" example:"refresh-token-example"`
}

// RegisterUser allows a user to register with their name, phone number, and password.
// @Summary register
// @Description This endpoint allows a user to register by providing their name, phone number, and password.
// @Tags user
// @Accept json
// @Produce json
// @Param request body RegisterUserRequest true "Registration parameters"
// @Success 201 {object} RegisterUserResponse "Successful registration"
// @Router /user/register [post]
func (h *UserHandler) RegisterUser(c echo.Context) error {
	var request RegisterUserRequest

	if err := c.Bind(&request); err != nil {
		common.HandleFailure(errors.Validation("Invalid request payload"), c)
		return nil
	}

	token, refreshToken, err := h.userService.RegisterUser(request.Name, request.PhoneNumber, request.Password)
	if err != nil {
		common.HandleFailure(err, c)
		return nil
	}

	response := RegisterUserResponse{
		Token:        token,
		RefreshToken: refreshToken,
	}

	return c.JSON(http.StatusCreated, response)
}
