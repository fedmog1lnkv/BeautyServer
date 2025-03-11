package staff

import (
	"beauty-server/internal/application/service/staff"
)

type StaffHandler struct {
	staffService *staff.StaffService
}

func NewStaffHandler(staffService *staff.StaffService) *StaffHandler {
	return &StaffHandler{
		staffService: staffService,
	}
}
