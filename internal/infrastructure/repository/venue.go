package repository

import (
	"beauty-server/internal/domain/entity"
	"beauty-server/internal/domain/repository"
	"beauty-server/internal/domain/value_object"
	"beauty-server/internal/infrastructure/model"
	"fmt"
	"github.com/google/uuid"
	"gorm.io/gorm"
)

type VenueRepository struct {
	DB *gorm.DB
}

func NewVenueRepository(db *gorm.DB) repository.VenueRepository {
	return &VenueRepository{DB: db}
}

func (r *VenueRepository) GetById(id uuid.UUID) (*entity.Venue, error) {
	var venueModel model.VenueModel
	err := r.DB.Where("id = ?", id).First(&venueModel).Error
	if err != nil {
		if err == gorm.ErrRecordNotFound {
			return nil, nil
		}
		return nil, fmt.Errorf("error fetching venue by id: %v", err)
	}

	venue, err := venueModel.ToDomain()
	if err != nil {
		return nil, err
	}

	return venue, nil
}

func (r *VenueRepository) GetAll(limit, offset int) ([]*entity.Venue, error) {
	var venueModels []model.VenueModel
	err := r.DB.Limit(limit).Offset(offset).Find(&venueModels).Error
	if err != nil {
		return nil, fmt.Errorf("error fetching venues with pagination: %v", err)
	}

	var venues []*entity.Venue
	for _, venueModel := range venueModels {
		venue, err := venueModel.ToDomain()
		if err != nil {
			return nil, fmt.Errorf("error converting venue model to domain: %v", err)
		}
		venues = append(venues, venue)
	}

	return venues, nil
}

func (r *VenueRepository) GetByLocation(location value_object.Location, limit, offset int) ([]*entity.Venue, error) {
	var venueModels []model.VenueModel

	query := `
		SELECT *, 
			(6371 * acos(
				cos(radians(?)) * cos(radians(latitude)) * cos(radians(longitude) - radians(?)) + 
				sin(radians(?)) * sin(radians(latitude))
			)) AS distance 
		FROM venue_models
		HAVING distance <= ? 
		ORDER BY distance 
		LIMIT ? OFFSET ?`

	err := r.DB.Raw(query, location.Latitude, location.Longitude, location.Latitude, 100, limit, offset).Scan(&venueModels).Error
	if err != nil {
		return nil, fmt.Errorf("error fetching venues by location: %v", err)
	}

	var venues []*entity.Venue
	for _, venueModel := range venueModels {
		venue, err := venueModel.ToDomain()
		if err != nil {
			return nil, fmt.Errorf("error converting venue model to domain: %v", err)
		}
		venues = append(venues, venue)
	}

	return venues, nil
}

func (r *VenueRepository) GetByOrganizationId(organizationID uuid.UUID) ([]*entity.Venue, error) {
	var venueModels []model.VenueModel
	err := r.DB.Where("organization_id = ?", organizationID).Find(&venueModels).Error
	if err != nil {
		return nil, fmt.Errorf("error fetching venues by organization id: %v", err)
	}

	var venues []*entity.Venue
	for _, venueModel := range venueModels {
		venue, err := venueModel.ToDomain()
		if err != nil {
			return nil, fmt.Errorf("error converting venue model to domain: %v", err)
		}
		venues = append(venues, venue)
	}

	return venues, nil
}

func (r *VenueRepository) Save(venue *entity.Venue) error {
	venueModel := model.FromDomainVenue(venue)
	if err := r.DB.Create(venueModel).Error; err != nil {
		return fmt.Errorf("failed to save venue: %v", err)
	}
	return nil
}

func (r *VenueRepository) Update(venue *entity.Venue) error {
	venueModel := model.FromDomainVenue(venue)
	if err := r.DB.Save(venueModel).Error; err != nil {
		return fmt.Errorf("failed to update venue: %v", err)
	}
	return nil
}

func (r *VenueRepository) Remove(venue *entity.Venue) error {
	venueModel := model.FromDomainVenue(venue)
	if err := r.DB.Delete(venueModel).Error; err != nil {
		return fmt.Errorf("failed to remove venue: %v", err)
	}
	return nil
}
