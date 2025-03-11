package organization

import (
	"beauty-server/internal/api/common"
	"beauty-server/internal/domain/errors"
	"github.com/labstack/echo/v4"
	"net/http"
)

// CreateOrganizationRequest represents the request body for creating an organization.
// swagger:parameters createOrganization
type CreateOrganizationRequest struct {
	Name string `json:"name" example:"Beauty Inc."`
}

// Create creates a new organization.
// @Summary Create an organization
// @Description Creates a new organization with the provided name
// @Tags admin
// @Accept json
// @Produce json
// @Param createOrganization body CreateOrganizationRequest true "Organization data to create"
// @Success 201 "Creation successful"
// @Router /organization [post]
func (h *OrganizationHandler) Create(c echo.Context) error {
	var request CreateOrganizationRequest

	if err := c.Bind(&request); err != nil {
		common.HandleFailure(errors.Validation("Invalid request payload"), c)
		return nil
	}

	err := h.organizationService.Create(request.Name)
	if err != nil {
		common.HandleFailure(err, c)
		return nil
	}

	return c.NoContent(http.StatusCreated)
}
