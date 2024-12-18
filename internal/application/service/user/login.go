package user

import (
	"beauty-server/internal/domain/value_object"
	"beauty-server/internal/infrastructure/auth"
	"fmt"
	"golang.org/x/crypto/bcrypt"
)

func (s *UserService) Login(phoneNumber, password string) (string, string, error) {
	userPhoneNumber, err := value_object.NewUserPhoneNumber(phoneNumber)
	if err != nil {
		return "", "", err
	}
	user, err := s.userRepo.GetByPhoneNumber(userPhoneNumber)
	if err != nil {
		return "", "", fmt.Errorf("user not found: %w", err)
	}

	err = bcrypt.CompareHashAndPassword([]byte(user.Password.Value()), []byte(password))
	if err != nil {
		return "", "", fmt.Errorf("incorrect password: %w", err)
	}

	accessToken, err := auth.GenerateToken(user.Id)
	if err != nil {
		return "", "", fmt.Errorf("error generating access token: %w", err)
	}

	refreshToken, err := auth.GenerateRefreshToken(user.Id)
	if err != nil {
		return "", "", fmt.Errorf("error generating refresh token: %w", err)
	}

	return accessToken, refreshToken, nil
}
