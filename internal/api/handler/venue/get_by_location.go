package venue

import (
	"beauty-server/internal/api/common"
	"beauty-server/internal/api/models"
	"beauty-server/internal/domain/entity"
	"github.com/labstack/echo/v4"
	"net/http"
	"strconv"
)

type GetVenueLookup struct {
	Id          string             `json:"id"`
	Name        string             `json:"name"`
	Description *string            `json:"description,omitempty"`
	Color       *int               `json:"color,omitempty"`
	Photo       *string            `json:"photo,omitempty"`
	Location    models.LocationDto `json:"location"`
}

func MapVenueToLookup(venue *entity.Venue) GetVenueLookup {
	var response GetVenueLookup
	response.Id = venue.Id.String()
	response.Name = venue.Name.Value()

	response.Location = models.LocationDto{
		Latitude:  venue.Location.Latitude,
		Longitude: venue.Location.Longitude,
	}

	if venue.Description != nil {
		desc := venue.Description.Value()
		response.Description = &desc
	}
	if venue.Theme.GetPhoto() != nil {
		photo := venue.Theme.GetPhoto()
		response.Photo = photo
	}

	return response
}

func (h *VenueHandler) GetByLocation(c echo.Context) error {
	latitudeParam := c.QueryParam("latitude")
	longitudeParam := c.QueryParam("longitude")
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

	latitude, err := strconv.ParseFloat(latitudeParam, 64)
	if err != nil {
		return c.JSON(400, map[string]string{"error": "invalid latitude"})
	}
	longitude, err := strconv.ParseFloat(longitudeParam, 64)
	if err != nil {
		return c.JSON(400, map[string]string{"error": "invalid longitude"})
	}

	venues, err := h.venueService.GetByLocation(latitude, longitude, limit, offset)
	if err != nil {
		common.HandleFailure(err, c)
		return nil
	}

	response := make([]GetVenueLookup, 0, len(venues))
	for _, venue := range venues {
		responseItem := MapVenueToLookup(venue)
		response = append(response, responseItem)
	}

	c.Response().Header().Set("Include-Nulls", "true")
	return c.JSON(http.StatusOK, response)
}
