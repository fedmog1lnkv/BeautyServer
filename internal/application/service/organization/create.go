package organization

import (
	"beauty-server/internal/domain/entity"
	"github.com/google/uuid"
)

func (s *OrganizationService) Create(name string) error {
	id := uuid.New()

	newOrganization, err := entity.NewOrganization(id, name)
	if err != nil {
		return err
	}

	err = s.organizationRepo.Save(newOrganization)
	if err != nil {
		return err
	}

	return nil
}
