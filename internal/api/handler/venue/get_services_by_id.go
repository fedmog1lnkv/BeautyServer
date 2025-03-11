package venue

import (
	"beauty-server/internal/api/common"
	"beauty-server/internal/domain/entity"
	"github.com/google/uuid"
	"github.com/labstack/echo/v4"
	"net/http"
)

// GetServiceByVenueVm represents the view model for a service in a venue.
// swagger:model GetServiceByVenueVm
type GetServiceByVenueVm struct {
	Id          string   `json:"id"`
	Name        string   `json:"name"`
	Description *string  `json:"description,omitempty"`
	Duration    *int     `json:"duration,omitempty"`
	Price       *float64 `json:"price,omitempty"`
}

func MapServiceToServiceByVenueVm(service *entity.Service) GetServiceByVenueVm {
	var response GetServiceByVenueVm
	response.Id = service.Id.String()
	response.Name = service.Name.Value()

	if service.Description != nil {
		desc := service.Description.Value()
		response.Description = &desc
	}

	if service.Duration != nil {
		minutes := int(service.Duration.Minutes())
		response.Duration = &minutes
	}

	if service.Price != nil {
		price := service.Price.Value()
		response.Price = &price
	}

	return response
}

// GetServicesById retrieves services by venue ID.
// @Summary Get services by venue ID
// @Description This endpoint retrieves services associated with a specific venue identified by its ID.
// @Tags venue
// @Accept json
// @Produce json
// @Param id query string true "Venue ID"
// @Success 200 {array} GetServiceByVenueVm "List of services for the venue"
// @Router /venue/services [get]
func (h *VenueHandler) GetServicesById(c echo.Context) error {
	id, err := uuid.Parse(c.Param("id"))

	services, err := h.venueService.GetServicesById(id)
	if err != nil {
		common.HandleFailure(err, c)
		return nil
	}

	response := make([]GetServiceByVenueVm, 0, len(services))
	for _, service := range services {
		responseItem := MapServiceToServiceByVenueVm(service)
		response = append(response, responseItem)
	}

	c.Response().Header().Set("Include-Nulls", "true")
	return c.JSON(http.StatusOK, response)
}
