package organization

import (
	"beauty-server/internal/api/common"
	"beauty-server/internal/domain/entity"
	"github.com/labstack/echo/v4"
	"net/http"
	"strconv"
)

// GetOrganizationLookup represents the organization data in the response.
// swagger:response GetOrganizationLookupResponse
type GetOrganizationLookup struct {
	ID          string  `json:"id"`
	Name        string  `json:"name"`
	Description *string `json:"description,omitempty"`
	Color       *int    `json:"color,omitempty"`
	Photo       *string `json:"photo,omitempty"`
}

func MapOrganizationToLookup(organization *entity.Organization) GetOrganizationLookup {
	var response GetOrganizationLookup
	response.ID = organization.Id.String()
	response.Name = organization.Name.Value()

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

// GetAll gets all organizations with pagination support.
// @Summary Get all organizations
// @Description Retrieve a list of organizations with pagination support
// @Tags organization
// @Accept json
// @Produce json
// @Param limit query int false "Number of organizations to return" default(10)
// @Param offset query int false "Number of organizations to skip" default(0)
// @Success 200 {array} GetOrganizationLookup "List of organizations"
// @Router /organization [get]
func (h *OrganizationHandler) GetAll(c echo.Context) error {
	limitParam := c.QueryParam("limit")
	offsetParam := c.QueryParam("offset")

	limit, err := strconv.Atoi(limitParam)
	if err != nil || limit <= 0 {
		limit = 10
	}

	offset, err := strconv.Atoi(offsetParam)
	if err != nil || offset < 0 {
		offset = 0
	}

	organizations, err := h.organizationService.GetAll(limit, offset)
	if err != nil {
		common.HandleFailure(err, c)
		return nil
	}

	response := make([]GetOrganizationLookup, 0, len(organizations))
	for _, org := range organizations {
		responseItem := MapOrganizationToLookup(org)
		response = append(response, responseItem)
	}

	c.Response().Header().Set("Include-Nulls", "true")
	return c.JSON(http.StatusOK, response)
}
