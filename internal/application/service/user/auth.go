package user

import (
	"beauty-server/internal/domain/value_object"
	"beauty-server/internal/infrastructure/auth"
	"fmt"
	"os"
	"strings"
	"time"
)

func (s *UserService) Auth(phoneNumber, code string) (string, string, error) {
	userPhoneNumber, err := value_object.NewUserPhoneNumber(phoneNumber)
	if err != nil {
		return "", "", err
	}

	phoneChallenge, err := s.phoneChallengeRepo.GetByPhoneNumber(userPhoneNumber.Value())
	if err != nil {
		return "", "", err
	}

	if phoneChallenge.ExpiredAt.Before(time.Now()) {
		return "", "", fmt.Errorf("Code expired")
	}

	if code != phoneChallenge.VerificationCode {
		return "", "", fmt.Errorf("Code invalid!")
	}

	user, err := s.userRepo.GetByPhoneNumber(userPhoneNumber)
	if err != nil {
		return "", "", fmt.Errorf("user not found: %w", err)
	}

	adminPhones := os.Getenv("ADMIN_PHONE_NUMBERS")
	isAdmin := false
	for _, phoneNumber := range strings.Split(adminPhones, ",") {
		adminPhoneNumber, err := value_object.NewUserPhoneNumber(phoneNumber)
		if err != nil {
			return "", "", err
		}
		if userPhoneNumber.Equal(adminPhoneNumber) {
			isAdmin = true
			break
		}
	}

	accessToken, err := auth.GenerateToken(user.Id, isAdmin)
	if err != nil {
		return "", "", fmt.Errorf("error generating access token: %w", err)
	}

	refreshToken, err := auth.GenerateRefreshToken(user.Id, isAdmin)
	if err != nil {
		return "", "", fmt.Errorf("error generating refresh token: %w", err)
	}

	err = s.phoneChallengeRepo.Remove(phoneChallenge)
	if err != nil {
		return "", "", err
	}

	return accessToken, refreshToken, nil
}
