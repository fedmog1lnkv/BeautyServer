package venue

import "beauty-server/internal/application/service/venue"

type VenueHandler struct {
	venueService *venue.VenueService
}

func NewVenueHandler(venueService *venue.VenueService) *VenueHandler {
	return &VenueHandler{
		venueService: venueService,
	}
}
