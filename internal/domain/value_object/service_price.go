package value_object

import (
	"beauty-server/internal/domain/errors"
	"fmt"
)

type ServicePrice struct {
	value float64
}

func NewServicePrice(price float64) (ServicePrice, error) {
	if price <= 0 {
		return ServicePrice{}, errors.NewErrServicePriceInvalid()
	}

	return ServicePrice{value: price}, nil
}

func (p ServicePrice) Value() float64 {
	return p.value
}

func (p ServicePrice) Equal(other ServicePrice) bool {
	return p.value == other.value
}

func (p ServicePrice) String() string {
	return fmt.Sprintf("%.2f", p.value)
}
