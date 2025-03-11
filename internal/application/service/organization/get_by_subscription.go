package organization

import (
	"beauty-server/internal/domain/entity"
	"beauty-server/internal/domain/enum"
	"fmt"
)

func (s *OrganizationService) GetBySubscription(limit, offset int, subscriptionType enum.OrganizationSubscription) ([]*entity.Organization, error) {
	var organizations []*entity.Organization
	var err error

	if subscriptionType == enum.All {
		organizations, err = s.organizationRepo.GetAll(limit, offset)
	} else {
		organizations, err = s.organizationRepo.GetBySubscription(limit, offset, subscriptionType)
	}

	if err != nil {
		return nil, fmt.Errorf("failed to get organizations: %w", err)
	}

	return organizations, nil
}
