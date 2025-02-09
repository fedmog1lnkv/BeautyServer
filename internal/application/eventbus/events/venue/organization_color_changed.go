package venue

import (
	"github.com/google/uuid"
)

type OrganizationColorChangedEvent struct {
	OrganizationID uuid.UUID
	OldColor       string
	NewColor       string
}

func (e OrganizationColorChangedEvent) Type() string {
	return "OrganizationColorChangedEvent"
}
