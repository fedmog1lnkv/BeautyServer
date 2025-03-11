package staff

import (
	"beauty-server/internal/domain/value_object"
	"beauty-server/internal/infrastructure/auth"
	"fmt"
	"time"
)

func (s *StaffService) Auth(phoneNumber, code string) (string, string, error) {
	staffNumber, err := value_object.NewStaffPhoneNumber(phoneNumber)
	if err != nil {
		return "", "", err
	}
	phoneChallenge, err := s.phoneChallengeRepo.GetByPhoneNumber(staffNumber.Value())
	if err != nil {
		return "", "", err
	}

	if phoneChallenge.ExpiredAt.Before(time.Now()) {
		return "", "", fmt.Errorf("Code expired")
	}

	if code != phoneChallenge.VerificationCode {
		return "", "", fmt.Errorf("Code invalid!")
	}

	staffPhoneNumber, err := value_object.NewStaffPhoneNumber(phoneNumber)
	if err != nil {
		return "", "", err
	}
	staff, err := s.staffRepo.GetByPhoneNumber(staffPhoneNumber)
	if err != nil {
		return "", "", fmt.Errorf("staff not found: %w", err)
	}

	accessToken, err := auth.GenerateToken(staff.Id, false)
	if err != nil {
		return "", "", fmt.Errorf("error generating access token: %w", err)
	}

	refreshToken, err := auth.GenerateRefreshToken(staff.Id, false)
	if err != nil {
		return "", "", fmt.Errorf("error generating refresh token: %w", err)
	}

	err = s.phoneChallengeRepo.Remove(phoneChallenge)
	if err != nil {
		return "", "", err
	}

	return accessToken, refreshToken, nil
}
