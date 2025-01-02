package organization

import (
	"beauty-server/internal/api/common"
	"beauty-server/internal/domain/entity"
	"github.com/google/uuid"
	"github.com/labstack/echo/v4"
	"net/http"
)

// GetOrganizationVm represents the detailed organization data in the response.
// swagger:response GetOrganizationVmResponse
type GetOrganizationVm struct {
	ID           string  `json:"id"`
	Name         string  `json:"name"`
	Description  *string `json:"description,omitempty"`
	Subscription string  `json:"subscription"`
	Color        *int    `json:"color,omitempty"`
	Photo        *string `json:"photo,omitempty"`
}

func MapOrganizationToVm(organization *entity.Organization) GetOrganizationVm {
	var response GetOrganizationVm
	response.ID = organization.Id.String()
	response.Name = organization.Name.Value()
	response.Subscription = organization.Subscription.String()

	if organization.Description != nil {
		desc := organization.Description.Value()
		response.Description = &desc
	}
	if organization.Color != nil {
		color := organization.Color.Value()
		response.Color = &color
	}
	if organization.Photo != nil {
		photo := organization.Photo.Value()
		response.Photo = &photo
	}

	return response
}

// GetById gets an organization by its unique ID.
// @Summary Get organization by ID
// @Description Retrieve an organization by its unique identifier
// @Tags organization
// @Accept json
// @Produce json
// @Param id path string true "Organization ID"
// @Success 200 {object} GetOrganizationVm "Organization details"
// @Router /organization/{id} [get]
func (h *OrganizationHandler) GetById(c echo.Context) error {
	id, err := uuid.Parse(c.Param("id"))

	organization, err := h.organizationService.GetById(id)
	if err != nil {
		common.HandleFailure(err, c)
		return nil
	}

	if organization == nil {
		return c.JSON(http.StatusNotFound, "Organization not found")
	}

	response := MapOrganizationToVm(organization)

	c.Response().Header().Set("Include-Nulls", "true")
	return c.JSON(http.StatusOK, response)
}
