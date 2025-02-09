package entity

import (
	"github.com/google/uuid"
	"time"
)

type PhoneChallenge struct {
	Id               uuid.UUID
	CreatedAt        time.Time
	PhoneNumber      string
	VerificationCode string
	ExpiredAt        time.Time
}

func NewPhoneChallenge(id uuid.UUID, phoneNumber, verificationCode string, duration time.Duration) (*PhoneChallenge, error) {
	timeNow := time.Now()

	return &PhoneChallenge{
		Id:               id,
		CreatedAt:        timeNow,
		PhoneNumber:      phoneNumber,
		VerificationCode: verificationCode,
		ExpiredAt:        timeNow.Add(duration),
	}, nil
}
