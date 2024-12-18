package common

import (
	domainErrors "beauty-server/internal/domain/errors"
	"errors"
	"github.com/labstack/echo/v4"
)

func HandleFailure(err error, c echo.Context) {
	var customErr *domainErrors.CustomError

	if ok := errors.As(err, &customErr); !ok {
		customErr = domainErrors.Internal(err.Error())
	}

	response := map[string]interface{}{
		"error": map[string]interface{}{
			"type":    customErr.Type,
			"message": customErr.Message,
		},
	}

	c.JSON(customErr.Code, response)
}
