package user

import (
	"beauty-server/internal/infrastructure/auth"
	"fmt"
	"github.com/google/uuid"
)

func (s *UserService) RefreshToken(refreshToken string) (string, string, error) {
	claims, err := auth.ParseToken(refreshToken)
	if err != nil {
		return "", "", fmt.Errorf("invalid refresh token: %w", err)
	}

	userID, err := uuid.Parse(claims.UserID)
	if err != nil {
		return "", "", fmt.Errorf("invalid user ID in refresh token: %w", err)
	}

	user, err := s.userRepo.GetById(userID)
	if err != nil {
		return "", "", fmt.Errorf("user not found: %w", err)
	}

	accessToken, err := auth.GenerateToken(user.Id)
	if err != nil {
		return "", "", fmt.Errorf("error generating access token: %w", err)
	}

	newRefreshToken, err := auth.GenerateRefreshToken(user.Id)
	if err != nil {
		return "", "", fmt.Errorf("error generating refresh token: %w", err)
	}

	return accessToken, newRefreshToken, nil
}
