package entity

import (
	"beauty-server/internal/domain/errors"
	"beauty-server/internal/domain/value_object"
	"github.com/google/uuid"
	"time"
)

type TimeSlot struct {
	Id        uuid.UUID
	StaffId   uuid.UUID
	VenueId   uuid.UUID
	Weekday   time.Weekday
	Intervals []value_object.Interval
}

type VenueRepository interface {
	GetById(id uuid.UUID) (*Venue, error)
}

func NewTimeSlot(id, staffId, venueId uuid.UUID, weekday time.Weekday, venueRepository VenueRepository) (*TimeSlot, error) {
	venue, err := venueRepository.GetById(venueId)
	if err != nil {
		return nil, err
	}
	if venue == nil {
		return nil, errors.NewErrVenueNotFound(venueId)
	}

	return &TimeSlot{
		Id:        id,
		StaffId:   staffId,
		VenueId:   venue.Id,
		Weekday:   weekday,
		Intervals: make([]value_object.Interval, 0),
	}, nil
}

func (ts TimeSlot) UpdateIntervals(intervals []value_object.Interval) error {
	for i := 0; i < len(intervals); i++ {
		for j := i + 1; j < len(intervals); j++ {
			if intervals[i].Overlaps(intervals[j]) {
				return errors.NewErrIntervalsOverlap(
					intervals[i].Start, intervals[i].End,
					intervals[j].Start, intervals[j].End,
				)
			}
		}
	}

	ts.Intervals = intervals
	return nil
}
