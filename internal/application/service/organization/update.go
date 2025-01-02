package organization

import (
	"beauty-server/internal/domain/entity"
	"beauty-server/internal/domain/enum"
	"fmt"
	"github.com/google/uuid"
)

type UpdateOrganizationModel struct {
	Id           uuid.UUID
	Name         *string
	Description  *string
	Subscription *string
	Color        *int
	Photo        *string
}

func (s *OrganizationService) Update(id uuid.UUID, request UpdateOrganizationModel) (*entity.Organization, error) {
	organization, err := s.organizationRepo.GetById(id)
	if err != nil {
		return nil, fmt.Errorf("failed to get organization with id: %w", err)
	}

	if organization == nil {
		return nil, fmt.Errorf("organization not found")
	}

	if request.Name != nil {
		if err := organization.UpdateName(*request.Name); err != nil {
			return nil, err
		}
	}

	if request.Description != nil {
		if err := organization.UpdateDescription(*request.Description); err != nil {
			return nil, err
		}
	}

	if request.Subscription != nil {
		subscription, err := enum.ParseOrganizationSubscription(*request.Subscription)
		if err != nil {
			return nil, fmt.Errorf("invalid subscription value: %w", err)
		}
		organization.UpdateSubscription(subscription)
	}

	if request.Color != nil {
		if err := organization.UpdateColor(*request.Color); err != nil {
			return nil, err
		}
	}

	if request.Photo != nil {
		if err := organization.UpdatePhoto(*request.Photo); err != nil {
			return nil, err
		}
	}

	// Сохраняем обновленную организацию в репозитории
	if err := s.organizationRepo.Update(organization); err != nil {
		return nil, fmt.Errorf("failed to update organization: %w", err)
	}

	return organization, nil
}
