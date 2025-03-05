package staff

import "beauty-server/internal/domain/repository"

type StaffService struct {
	staffRepo          repository.StaffRepository
	organizationRepo   repository.OrganizationRepository
	phoneChallengeRepo repository.PhoneChallengeRepository
}

func NewStaffService(staffRepo repository.StaffRepository, organizationRepo repository.OrganizationRepository, phoneChallengeRepo repository.PhoneChallengeRepository) *StaffService {
	return &StaffService{
		staffRepo:          staffRepo,
		organizationRepo:   organizationRepo,
		phoneChallengeRepo: phoneChallengeRepo,
	}
}
