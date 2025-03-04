package repository

import (
	"beauty-server/internal/domain/entity"
	"beauty-server/internal/domain/repository"
	"beauty-server/internal/infrastructure/model"
	"fmt"
	"github.com/google/uuid"
	"gorm.io/gorm"
)

type ServiceRepository struct {
	DB *gorm.DB
}

func NewServiceRepository(db *gorm.DB) repository.ServiceRepository {
	return &ServiceRepository{DB: db}
}

func (s *ServiceRepository) ExistsService(id uuid.UUID) bool {
	var count int64
	err := s.DB.Model(&model.ServiceModel{}).Where("id = ?", id).Count(&count).Error
	if err != nil {
		return false
	}
	return count > 0
}

func (s ServiceRepository) Save(service *entity.Service) error {
	serviceModel := model.FromDomainService(service)
	if err := s.DB.Create(serviceModel).Error; err != nil {
		return fmt.Errorf("failed to save service: %v", err)
	}
	return nil
}

func (s ServiceRepository) Update(service *entity.Service) error {
	serviceModel := model.FromDomainService(service)
	if err := s.DB.Save(serviceModel).Error; err != nil {
		return fmt.Errorf("failed to update service: %v", err)
	}
	return nil
}

func (s ServiceRepository) Remove(service *entity.Service) error {
	serviceModel := model.FromDomainService(service)
	if err := s.DB.Delete(serviceModel).Error; err != nil {
		return fmt.Errorf("failed to remove service: %v", err)
	}
	return nil
}

func (s ServiceRepository) GetByOrganizationId(organizationId uuid.UUID) ([]*entity.Service, error) {
	var serviceModels []model.ServiceModel
	err := s.DB.Where("organization_id = ?", organizationId).Find(&serviceModels).Error
	if err != nil {
		return nil, fmt.Errorf("error fetching venues by organization id: %v", err)
	}

	var services []*entity.Service
	for _, serviceModel := range serviceModels {
		service, err := serviceModel.ToDomain()
		if err != nil {
			return nil, fmt.Errorf("error converting service model to domain: %v", err)
		}
		services = append(services, service)
	}

	return services, nil
}
