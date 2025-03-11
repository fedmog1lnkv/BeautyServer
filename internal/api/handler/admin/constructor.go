package admin

import "beauty-server/internal/application/service/organization"

type AdminHandler struct {
	organizationService *organization.OrganizationService
}

func NewAdminHandler(organizationService *organization.OrganizationService) *AdminHandler {
	return &AdminHandler{
		organizationService: organizationService,
	}
}
