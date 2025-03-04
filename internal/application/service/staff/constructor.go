package staff

import "beauty-server/internal/domain/repository"

type StaffService struct {
	staffRepo          repository.StaffRepository
	phoneChallengeRepo repository.PhoneChallengeRepository
}

func NewStaffService(staffRepo repository.StaffRepository, phoneChallengeRepo repository.PhoneChallengeRepository) *StaffService {
	return &StaffService{
		staffRepo:          staffRepo,
		phoneChallengeRepo: phoneChallengeRepo,
	}
}
