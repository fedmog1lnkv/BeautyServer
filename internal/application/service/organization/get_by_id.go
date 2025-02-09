package organization

import (
	"beauty-server/internal/domain/entity"
	"fmt"
	"github.com/google/uuid"
)

func (s *OrganizationService) GetById(id uuid.UUID) (*entity.Organization, error) {
	organization, err := s.organizationRepo.GetByIdWithVenues(id)
	if err != nil {
		return nil, fmt.Errorf("failed to get organization with id: %w", err)
	}
	if organization == nil {
		return nil, fmt.Errorf("organization with id %v not found", id)
	}
	return organization, nil
}
