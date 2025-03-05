package venue

import (
	"beauty-server/internal/api/common"
	"beauty-server/internal/domain/entity"
	"github.com/google/uuid"
	"github.com/labstack/echo/v4"
	"net/http"
)

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

	return response
}

func (h *VenueHandler) GetServicesById(c echo.Context) error {
	idParam := c.QueryParam("id")

	if idParam == "" {
		return c.JSON(http.StatusBadRequest, map[string]string{"error": "id is required"})
	}

	id, err := uuid.Parse(idParam)
	if err != nil {
		common.HandleFailure(err, c)
		return nil
	}

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
