package user

import (
	"beauty-server/internal/domain/entity"
	"beauty-server/internal/domain/value_object"
	"fmt"
	"github.com/google/uuid"
	"math/rand"
	"time"
)

func (s *UserService) GeneratePhoneChallenge(phoneNumber string) (bool, error) {
	userPhoneNumber, err := value_object.NewUserPhoneNumber(phoneNumber)
	if err != nil {
		return false, err
	}

	user, err := s.userRepo.GetByPhoneNumber(userPhoneNumber)
	if err != nil {
		return false, err
	}

	if user == nil {
		isSuccess, userName, err := s.phoneChallengeRepo.SendAuthRequest(userPhoneNumber.Value())
		if err != nil {
			return false, fmt.Errorf("error while auth request: %w", err)
		}
		if !isSuccess {
			return false, fmt.Errorf("user reject auth")
		}

		userId := uuid.New()
		newUser, err := entity.NewUser(userId, userName, phoneNumber, s.userRepo)
		if err != nil {
			return false, err
		}
		err = s.userRepo.Save(newUser)
		if err != nil {
			return false, err
		}

		user = newUser
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
	newPhoneChallenge, err := entity.NewPhoneChallenge(id, user.PhoneNumber.Value(), code, 5*time.Minute)
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
