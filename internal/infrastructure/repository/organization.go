package repository

import (
	"beauty-server/internal/domain/entity"
	"beauty-server/internal/domain/repository"
	"beauty-server/internal/infrastructure/model"
	"fmt"
	"github.com/google/uuid"
	"gorm.io/gorm"
)

type OrganizationRepository struct {
	DB *gorm.DB
}

func NewOrganizationRepository(db *gorm.DB) repository.OrganizationRepository {
	return &OrganizationRepository{DB: db}
}

func (r *OrganizationRepository) GetById(id uuid.UUID) (*entity.Organization, error) {
	var orgModel model.OrganizationModel
	err := r.DB.Where("id = ?", id).First(&orgModel).Error
	if err != nil {
		if err == gorm.ErrRecordNotFound {
			return nil, nil
		}
		return nil, fmt.Errorf("error fetching organization by id: %v", err)
	}

	org, err := orgModel.ToDomain()
	if err != nil {
		return nil, err
	}

	return org, nil
}

func (r *OrganizationRepository) GetByIdWithVenues(id uuid.UUID) (*entity.Organization, error) {
	var orgModel model.OrganizationModel
	err := r.DB.Where("id = ?", id).First(&orgModel).Error
	if err != nil {
		if err == gorm.ErrRecordNotFound {
			return nil, nil
		}
		return nil, fmt.Errorf("error fetching organization by id: %v", err)
	}

	var venueIds []uuid.UUID
	err = r.DB.Model(&model.VenueModel{}).Where("organization_id = ?", id).Pluck("id", &venueIds).Error
	if err != nil {
		return nil, fmt.Errorf("error fetching venue ids: %v", err)
	}

	org, err := orgModel.ToDomain()
	if err != nil {
		return nil, err
	}

	org.VenueIds = venueIds

	return org, nil
}

func (r *OrganizationRepository) GetAll(limit, offset int) ([]*entity.Organization, error) {
	var orgModels []model.OrganizationModel
	err := r.DB.Limit(limit).Offset(offset).Find(&orgModels).Error
	if err != nil {
		return nil, fmt.Errorf("error fetching organizations with pagination: %v", err)
	}

	var organizations []*entity.Organization
	for _, orgModel := range orgModels {
		org, err := orgModel.ToDomain()
		if err != nil {
			return nil, fmt.Errorf("error converting organization model to domain: %v", err)
		}
		organizations = append(organizations, org)
	}

	return organizations, nil
}

func (r *OrganizationRepository) Save(org *entity.Organization) error {
	orgModel := model.FromDomainOrganization(org)
	if err := r.DB.Create(orgModel).Error; err != nil {
		return fmt.Errorf("failed to save organization: %v", err)
	}
	return nil
}

func (r *OrganizationRepository) Update(org *entity.Organization) error {
	orgModel := model.FromDomainOrganization(org)
	if err := r.DB.Save(orgModel).Error; err != nil {
		return fmt.Errorf("failed to update organization: %v", err)
	}
	return nil
}

func (r *OrganizationRepository) Remove(organization *entity.Organization) error {
	orgModel := model.FromDomainOrganization(organization)
	if err := r.DB.Delete(orgModel).Error; err != nil {
		return fmt.Errorf("failed to remove organization: %v", err)
	}
	return nil
}

func (r *OrganizationRepository) Exists(organizationId uuid.UUID) (bool, error) {
	var count int64
	err := r.DB.Model(&model.OrganizationModel{}).Where("id = ?", organizationId).Count(&count).Error
	if err != nil {
		return false, fmt.Errorf("error checking existence of organization by id: %v", err)
	}
	return count > 0, nil
}
