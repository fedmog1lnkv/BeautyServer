package venue

import "github.com/google/uuid"

type OrganizationPhotoChangedEvent struct {
	OrganizationID uuid.UUID
	OldPhoto       *string
	NewPhoto       string
}

func (e OrganizationPhotoChangedEvent) Type() string {
	return "OrganizationPhotoChangedEvent"
}
