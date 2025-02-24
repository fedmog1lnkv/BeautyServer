package service

import "beauty-server/internal/domain/repository"

type ServiceService struct {
	serviceRepo      repository.ServiceRepository
	organizationRepo repository.OrganizationRepository
}

func NewServiceService(serviceRepo repository.ServiceRepository, organizationRepo repository.OrganizationRepository) *ServiceService {
	return &ServiceService{
		serviceRepo:      serviceRepo,
		organizationRepo: organizationRepo,
	}
}
