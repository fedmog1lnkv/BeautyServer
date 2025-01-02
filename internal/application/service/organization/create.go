package organization

import (
	"beauty-server/internal/domain/entity"
	"fmt"
	"github.com/google/uuid"
)

func (s *OrganizationService) Create(name string) error {
	id := uuid.New()

	newOrganization, err := entity.NewOrganization(id, name)
	if err != nil {
		return fmt.Errorf("error creating organization: %w", err)
	}

	err = s.organizationRepo.Save(newOrganization)
	if err != nil {
		return fmt.Errorf("error creating organization: %w", err)
	}

	return nil
}
