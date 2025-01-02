package organization

import "beauty-server/internal/domain/repository"

type OrganizationService struct {
	organizationRepo repository.OrganizationRepository
}

func NewOrganizationService(organizationRepo repository.OrganizationRepository) *OrganizationService {
	return &OrganizationService{
		organizationRepo: organizationRepo,
	}
}
