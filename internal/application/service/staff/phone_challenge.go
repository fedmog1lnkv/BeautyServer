package staff

import (
	"beauty-server/internal/domain/entity"
	"beauty-server/internal/domain/value_object"
	"fmt"
	"github.com/google/uuid"
	"math/rand"
	"time"
)

func (s *StaffService) GeneratePhoneChallenge(phoneNumber string, organizationId uuid.UUID) (bool, error) {
	staffPhoneNumber, err := value_object.NewStaffPhoneNumber(phoneNumber)
	if err != nil {
		return false, err
	}

	staff, err := s.staffRepo.GetByPhoneNumber(staffPhoneNumber)
	if err != nil {
		return false, err
	}

	if staff == nil {
		isSuccess, staffName, err := s.phoneChallengeRepo.SendAuthRequest(staffPhoneNumber.Value())
		if err != nil {
			return false, fmt.Errorf("error while auth request: %w", err)
		}
		if !isSuccess {
			return false, fmt.Errorf("staff reject auth")
		}

		userId := uuid.New()
		newStaff, err := entity.NewStaff(userId, organizationId, staffName, phoneNumber, s.staffRepo, s.organizationRepo)
		if err != nil {
			return false, err
		}
		err = s.staffRepo.Save(newStaff)
		if err != nil {
			return false, err
		}

		staff = newStaff
	}

	oldPhoneChallenge, err := s.phoneChallengeRepo.GetByPhoneNumber(phoneNumber)
	if err != nil {
		return false, err
	}
	if oldPhoneChallenge != nil {
		err = s.phoneChallengeRepo.Remove(oldPhoneChallenge)
		if err != nil {
			return false, err
		}
	}

	code := fmt.Sprintf("%06d", rand.Intn(1000000))

	id := uuid.New()
	newPhoneChallenge, err := entity.NewPhoneChallenge(id, staff.PhoneNumber.Value(), code, 5*time.Minute)
	err = s.phoneChallengeRepo.Save(newPhoneChallenge)
	if err != nil {
		return false, fmt.Errorf("error generating code: %w", err)
	}

	err = s.phoneChallengeRepo.SendCode(newPhoneChallenge.PhoneNumber, newPhoneChallenge.VerificationCode)
	if err != nil {
		return false, fmt.Errorf("error sending code: %w", err)
	}

	return true, nil
}
