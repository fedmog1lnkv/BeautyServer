package model

import (
	"beauty-server/internal/domain/entity"
	"beauty-server/internal/domain/value_object"
	"encoding/json"
	"github.com/google/uuid"
	"gorm.io/datatypes"
	"time"
)

type TimeSlotModel struct {
	ID        uuid.UUID      `gorm:"type:uuid;primaryKey"`
	StaffID   uuid.UUID      `gorm:"type:uuid;not null"`
	VenueID   uuid.UUID      `gorm:"type:uuid;not null"`
	Weekday   time.Weekday   `gorm:"type:integer;not null"`
	Intervals datatypes.JSON `gorm:"type:jsonb"`

	Staff StaffModel `gorm:"foreignKey:StaffID;references:ID;constraint:OnDelete:CASCADE"`
	Venue VenueModel `gorm:"foreignKey:VenueID;references:ID"`
}

func (TimeSlotModel) TableName() string {
	return "time_slots"
}

func (m *TimeSlotModel) ToDomain() (*entity.TimeSlot, error) {
	var intervals []value_object.Interval
	if err := json.Unmarshal(m.Intervals, &intervals); err != nil {
		return nil, err
	}

	return &entity.TimeSlot{
		Id:        m.ID,
		StaffId:   m.StaffID,
		VenueId:   m.VenueID,
		Weekday:   m.Weekday,
		Intervals: intervals,
	}, nil
}

func FromDomainTimeSlot(timeSlot *entity.TimeSlot) *TimeSlotModel {
	intervals, err := json.Marshal(timeSlot.Intervals)
	if err != nil {
		return nil
	}

	return &TimeSlotModel{
		ID:        timeSlot.Id,
		StaffID:   timeSlot.StaffId,
		VenueID:   timeSlot.VenueId,
		Weekday:   timeSlot.Weekday,
		Intervals: intervals,
	}
}
