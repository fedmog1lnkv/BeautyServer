package venue

import (
	"beauty-server/internal/api/common"
	"beauty-server/internal/domain/errors"
	"github.com/google/uuid"
	"github.com/labstack/echo/v4"
	"net/http"
)

// CreateVenueRequest represents the request body for creating an venue.
// swagger:parameters createVenueRequest
type CreateVenueRequest struct {
	OrganizationId uuid.UUID `json:"organization_id"`
	Name           string    `json:"name" example:"on Wall Street"`
	Latitude       float64   `json:"latitude" example:"40.7128"`
	Longitude      float64   `json:"longitude" example:"-74.0060"`
}

func (h *VenueHandler) Create(c echo.Context) error {
	var request CreateVenueRequest

	if err := c.Bind(&request); err != nil {
		common.HandleFailure(errors.Validation("Invalid request payload"), c)
		return nil
	}

	err := h.venueService.Create(request.OrganizationId, request.Name, request.Latitude, request.Longitude)
	if err != nil {
		common.HandleFailure(err, c)
		return nil
	}

	return c.NoContent(http.StatusCreated)
}
