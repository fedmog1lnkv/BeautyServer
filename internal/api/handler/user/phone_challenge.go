package user

import (
	"beauty-server/internal/api/common"
	"beauty-server/internal/domain/errors"
	"github.com/labstack/echo/v4"
	"net/http"
)

// PhoneChallengeRequest represents the request body for initiating a phone challenge.
// swagger:parameters phoneChallenge
type PhoneChallengeRequest struct {
	PhoneNumber string `json:"phone_number" example:"+1234567890"`
}

// PhoneChallengeResponse represents the response body for a successful phone challenge initiation.
// swagger:response phoneChallengeResponse
type PhoneChallengeResponse struct {
	IsSuccess bool `json:"is_success" example:"true"`
}

// PhoneChallenge initiates a phone challenge by sending a verification code to the provided phone number.
// @Summary Initiate phone challenge
// @Description This endpoint sends a verification code to the given phone number to verify user identity.
// @Tags user
// @Accept json
// @Produce json
// @Param request body PhoneChallengeRequest true "Phone challenge request parameters"
// @Success 201 {object} PhoneChallengeResponse "Phone challenge initiated successfully"
// @Router /user/phone-challenge [post]
func (h *UserHandler) PhoneChallenge(c echo.Context) error {
	var request PhoneChallengeRequest

	if err := c.Bind(&request); err != nil {
		common.HandleFailure(errors.Validation("Invalid request payload"), c)
		return nil
	}

	isSuccess, err := h.userService.GeneratePhoneChallenge(request.PhoneNumber)
	if err != nil {
		common.HandleFailure(err, c)
		return nil
	}

	response := PhoneChallengeResponse{
		IsSuccess: isSuccess,
	}

	return c.JSON(http.StatusCreated, response)
}
