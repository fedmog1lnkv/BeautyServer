package organization

import (
	"beauty-server/internal/api/common"
	orgApplication "beauty-server/internal/application/service/organization"
	"beauty-server/internal/domain/errors"
	"github.com/google/uuid"
	"github.com/labstack/echo/v4"
	"net/http"
)

// UpdateOrganizationRequest represents the request body for updating an organization.
// swagger:parameters updateOrganization
type UpdateOrganizationRequest struct {
	Id           uuid.UUID `json:"id"`
	Name         *string   `json:"name,omitempty"`
	Description  *string   `json:"description,omitempty"`
	Subscription *string   `json:"subscription,omitempty"`
	Color        *string   `json:"color,omitempty"`
	Photo        *string   `json:"photo,omitempty"`
}

// Update is the handler function to update an organization's details.
// @Summary Update organization details
// @Description Update an existing organization's name, description, subscription, color, and photo.
// @Tags organization
// @Accept json
// @Produce json
// @Param request body UpdateOrganizationRequest true "Update Organization Request"
// @Success 200 "Organization updated successfully"
// @Router /organization [patch]
func (h *OrganizationHandler) Update(c echo.Context) error {
	var request UpdateOrganizationRequest
	if err := c.Bind(&request); err != nil {
		common.HandleFailure(errors.Validation("Invalid request payload"), c)
		return nil
	}

	updateModel := orgApplication.UpdateOrganizationModel{
		Id:           request.Id,
		Name:         request.Name,
		Description:  request.Description,
		Subscription: request.Subscription,
		Color:        request.Color,
		Photo:        request.Photo,
	}

	_, err := h.organizationService.Update(request.Id, updateModel)
	if err != nil {
		common.HandleFailure(err, c)
		return nil
	}

	return c.NoContent(http.StatusOK)
}
