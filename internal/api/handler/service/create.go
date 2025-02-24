package service

import (
	"beauty-server/internal/api/common"
	"beauty-server/internal/domain/errors"
	"github.com/google/uuid"
	"github.com/labstack/echo/v4"
	"net/http"
)

// CreateServiceRequest represents the request body for creating a service.
// swagger:parameters createServiceRequest
type CreateServiceRequest struct {
	OrganizationId uuid.UUID `json:"organization_id"`
	Name           string    `json:"name" example:"Haircut"`
}

// Create creates a new service with the provided details.
// @Summary Create a service
// @Description This endpoint allows the creation of a new service for an organization.
// @Tags service
// @Accept json
// @Produce json
// @Param request body CreateServiceRequest true "Service creation request parameters"
// @Success 201 "Service created successfully"
// @Router /service [post]
func (h *ServiceHandler) Create(c echo.Context) error {
	var request CreateServiceRequest

	if err := c.Bind(&request); err != nil {
		common.HandleFailure(errors.Validation("Invalid request payload"), c)
		return nil
	}

	err := h.serviceService.Create(request.OrganizationId, request.Name)
	if err != nil {
		common.HandleFailure(err, c)
		return nil
	}

	return c.NoContent(http.StatusCreated)
}
