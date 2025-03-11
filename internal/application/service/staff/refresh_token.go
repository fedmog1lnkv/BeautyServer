package staff

import (
	"beauty-server/internal/infrastructure/auth"
	"fmt"
	"github.com/google/uuid"
)

func (s *StaffService) RefreshToken(refreshToken string) (string, string, error) {
	claims, err := auth.ParseToken(refreshToken)
	if err != nil {
		return "", "", fmt.Errorf("invalid refresh token: %w", err)
	}

	userID, err := uuid.Parse(claims.UserID)
	if err != nil {
		return "", "", fmt.Errorf("invalid user Id in refresh token: %w", err)
	}

	user, err := s.staffRepo.GetById(userID)
	if err != nil {
		return "", "", fmt.Errorf("user not found: %w", err)
	}

	accessToken, err := auth.GenerateToken(user.Id, false)
	if err != nil {
		return "", "", fmt.Errorf("error generating access token: %w", err)
	}

	newRefreshToken, err := auth.GenerateRefreshToken(user.Id, false)
	if err != nil {
		return "", "", fmt.Errorf("error generating refresh token: %w", err)
	}

	return accessToken, newRefreshToken, nil
}
