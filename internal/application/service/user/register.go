package user

import (
	"beauty-server/internal/domain/entity"
	"beauty-server/internal/infrastructure/auth"
	"fmt"
	"github.com/google/uuid"
	"golang.org/x/crypto/bcrypt"
)

func (s *UserService) RegisterUser(name, phoneNumber, password string) (string, string, error) {
	id := uuid.New()

	hashedPassword, err := bcrypt.GenerateFromPassword([]byte(password), bcrypt.DefaultCost)
	if err != nil {
		return "", "", fmt.Errorf("error creating user: %w", err)
	}

	newUser, err := entity.NewUser(id, name, phoneNumber, string(hashedPassword), s.userRepo)
	if err != nil {
		return "", "", err
	}

	err = s.userRepo.Save(newUser)
	if err != nil {
		return "", "", fmt.Errorf("error saving user: %w", err)
	}

	token, err := auth.GenerateToken(newUser.Id)
	if err != nil {
		return "", "", fmt.Errorf("error generating access token: %w", err)
	}

	refreshToken, err := auth.GenerateRefreshToken(newUser.Id)
	if err != nil {
		return "", "", fmt.Errorf("error generating refresh token: %w", err)
	}

	return token, refreshToken, nil
}
