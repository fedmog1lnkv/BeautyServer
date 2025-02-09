package user

import "beauty-server/internal/domain/repository"

type UserService struct {
	userRepo           repository.UserRepository
	phoneChallengeRepo repository.PhoneChallengeRepository
}

func NewUserService(userRepo repository.UserRepository, phoneChallengeRepo repository.PhoneChallengeRepository) *UserService {
	return &UserService{
		userRepo:           userRepo,
		phoneChallengeRepo: phoneChallengeRepo,
	}
}
