package venue

import (
	"beauty-server/internal/api/common"
	"beauty-server/internal/api/models"
	"beauty-server/internal/domain/entity"
	"github.com/labstack/echo/v4"
	"net/http"
	"strconv"
)

// GetVenueLookup represents the venue lookup response.
// swagger:model GetVenueLookup
type GetVenueLookup struct {
	Id          string                 `json:"id"`
	Name        string                 `json:"name"`
	Description *string                `json:"description,omitempty"`
	Location    models.LocationDto     `json:"location"`
	Theme       VenueThemeConfigLookup `json:"theme"`
}

type VenueThemeConfigLookup struct {
	Color string  `json:"color"`
	Photo *string `json:"photo,omitempty"`
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
		response.Theme.Photo = photo
	}
	response.Theme.Color = venue.Theme.GetColor()

	return response
}

// GetByLocation retrieves venues based on location.
// @Summary Get venues by location
// @Description This endpoint retrieves venues filtered by latitude and longitude.
// @Tags venue
// @Accept json
// @Produce json
// @Param latitude query float64 true "Latitude"
// @Param longitude query float64 true "Longitude"
// @Param limit query int false "Limit of results (default: 10)"
// @Param offset query int false "Offset for pagination (default: 0)"
// @Success 200 {array} GetVenueLookup "List of venues"
// @Router /venue/by-location [get]
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
