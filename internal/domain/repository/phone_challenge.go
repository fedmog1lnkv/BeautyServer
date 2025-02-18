package repository

import "beauty-server/internal/domain/entity"

type PhoneChallengeRepository interface {
	Save(phoneChallenge *entity.PhoneChallenge) error
	GetByPhoneNumber(phoneNumber string) (*entity.PhoneChallenge, error)
	SendAuthRequest(phoneNumber string) (bool, string, error)
	SendCode(phoneNumber, code string) error
	Remove(phoneChallenge *entity.PhoneChallenge) error
}
