package user

import (
	"beauty-server/internal/domain/value_object"
	"beauty-server/internal/infrastructure/auth"
	"fmt"
	"time"
)

func (s *UserService) Auth(phoneNumber, code string) (string, string, error) {
	phoneChallenge, err := s.phoneChallengeRepo.GetByPhoneNumber(phoneNumber)
	if err != nil {
		return "", "", err
	}

	if phoneChallenge.ExpiredAt.Before(time.Now()) {
		return "", "", fmt.Errorf("Code expired")
	}

	if code != phoneChallenge.VerificationCode {
		return "", "", fmt.Errorf("code govno!")
	}

	userPhoneNumber, err := value_object.NewUserPhoneNumber(phoneNumber)
	if err != nil {
		return "", "", err
	}
	user, err := s.userRepo.GetByPhoneNumber(userPhoneNumber)
	if err != nil {
		return "", "", fmt.Errorf("user not found: %w", err)
	}

	accessToken, err := auth.GenerateToken(user.Id)
	if err != nil {
		return "", "", fmt.Errorf("error generating access token: %w", err)
	}

	refreshToken, err := auth.GenerateRefreshToken(user.Id)
	if err != nil {
		return "", "", fmt.Errorf("error generating refresh token: %w", err)
	}

	err = s.phoneChallengeRepo.Remove(phoneChallenge)
	if err != nil {
		return "", "", err
	}

	return accessToken, refreshToken, nil
}
