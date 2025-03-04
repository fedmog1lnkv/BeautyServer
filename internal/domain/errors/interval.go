package errors

import (
	"fmt"
	"time"
)

const (
	ErrIntervalStartBeforeEnd = "interval start %v must be before end %v"
	ErrIntervalsOverlap       = "intervals overlap: [%v - %v] and [%v - %v]"
)

func NewErrIntervalStartBeforeEnd(start, end time.Time) *CustomError {
	return Validation(fmt.Sprintf(ErrIntervalStartBeforeEnd, start, end))
}

func NewErrIntervalsOverlap(startA, endA, startB, endB time.Time) *CustomError {
	return Conflict(fmt.Sprintf(ErrIntervalsOverlap, startA, endA, startB, endB))
}
