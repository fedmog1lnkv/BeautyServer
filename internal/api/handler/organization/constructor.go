package organization

import (
	"beauty-server/internal/application/service/organization"
)

type OrganizationHandler struct {
	organizationService *organization.OrganizationService
}

func NewOrganizationHandler(organizationService *organization.OrganizationService) *OrganizationHandler {
	return &OrganizationHandler{
		organizationService: organizationService,
	}
}
