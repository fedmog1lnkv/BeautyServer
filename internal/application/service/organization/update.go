package organization

import (
	handlers "beauty-server/internal/application/eventbus/events/venue"
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
	Color        *string
	Photo        *string
}

func (s *OrganizationService) Update(id uuid.UUID, request UpdateOrganizationModel) (*entity.Organization, error) {
	organization, err := s.organizationRepo.GetByIdWithVenues(id)
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

	var oldColor string
	if request.Color != nil {
		oldColor = organization.Theme.GetColor()
		if err := organization.UpdateColor(*request.Color); err != nil {
			return nil, err
		}
	}

	var oldPhoto *string
	if request.Photo != nil {
		oldPhoto = organization.Theme.GetPhoto()
		if err := organization.UpdatePhoto(request.Photo); err != nil {
			return nil, err
		}
	}

	if request.Photo != nil {
		if err := organization.UpdatePhoto(request.Photo); err != nil {
			return nil, err
		}
	}

	if err := s.organizationRepo.Update(organization); err != nil {
		return nil, fmt.Errorf("failed to update organization: %w", err)
	}

	if request.Color != nil {
		event := handlers.OrganizationColorChangedEvent{
			OrganizationID: id,
			OldColor:       oldColor,
			NewColor:       *request.Color,
		}
		if err := s.eventBus.Publish(event); err != nil {
			return nil, fmt.Errorf("failed to publish event: %w", err)
		}
	}

	if request.Photo != nil {
		event := handlers.OrganizationPhotoChangedEvent{
			OrganizationID: id,
			OldPhoto:       oldPhoto,
			NewPhoto:       *request.Photo,
		}
		if err := s.eventBus.Publish(event); err != nil {
			return nil, fmt.Errorf("failed to publish event: %w", err)
		}
	}

	return organization, nil
}
