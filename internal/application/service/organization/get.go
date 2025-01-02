package organization

import (
	"beauty-server/internal/domain/entity"
	"fmt"
)

func (s *OrganizationService) GetAll(limit, offset int) ([]*entity.Organization, error) {
	organizations, err := s.organizationRepo.GetAll(limit, offset)
	if err != nil {
		return nil, fmt.Errorf("failed to get organizations: %w", err)
	}
	return organizations, nil
}
