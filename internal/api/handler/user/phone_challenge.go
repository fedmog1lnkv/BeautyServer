package user

import (
	"beauty-server/internal/api/common"
	"beauty-server/internal/domain/errors"
	"github.com/labstack/echo/v4"
	"net/http"
)

type PhoneChallengeRequest struct {
	PhoneNumber string `json:"phone_number" example:"+1234567890"`
}

type PhoneChallengeResponse struct {
	IsSuccess bool `json:"is_success" example:"true"`
}

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
