package model

import (
	"beauty-server/internal/domain/entity"
	"github.com/google/uuid"
	"time"
)

type PhoneChallengeModel struct {
	ID               uuid.UUID `gorm:"type:uuid;primaryKey"`
	CreatedAt        time.Time `gorm:"not null"`
	PhoneNumber      string    `gorm:"type:varchar(20);not null"`
	VerificationCode string    `gorm:"type:varchar(10);not null"`
	ExpiredAt        time.Time `gorm:"not null"`
}

func (PhoneChallengeModel) TableName() string {
	return "challenge_model"
}

func (m *PhoneChallengeModel) ToDomain() (*entity.PhoneChallenge, error) {
	return &entity.PhoneChallenge{
		Id:               m.ID,
		CreatedAt:        m.CreatedAt,
		PhoneNumber:      m.PhoneNumber,
		VerificationCode: m.VerificationCode,
		ExpiredAt:        m.ExpiredAt,
	}, nil
}

func FromDomainPhoneChallenge(phoneChallenge *entity.PhoneChallenge) *PhoneChallengeModel {
	return &PhoneChallengeModel{
		ID:               phoneChallenge.Id,
		CreatedAt:        phoneChallenge.CreatedAt,
		PhoneNumber:      phoneChallenge.PhoneNumber,
		VerificationCode: phoneChallenge.VerificationCode,
		ExpiredAt:        phoneChallenge.ExpiredAt,
	}
}
