package value_object

import (
	"beauty-server/internal/domain/errors"
	"time"
)

type Interval struct {
	Start time.Time
	End   time.Time
}

func NewInterval(start, end time.Time) (Interval, error) {
	if start.After(end) || start.Equal(end) {
		return Interval{}, errors.NewErrIntervalStartBeforeEnd(start, end)
	}
	return Interval{
		Start: start,
		End:   end,
	}, nil
}

func (i Interval) Overlaps(other Interval) bool {
	return i.Start.Before(other.End) && i.End.After(other.Start)
}

func (i Interval) Equal(other Interval) bool {
	return i.Start.Equal(other.End) && i.End.Equal(other.End)
}
