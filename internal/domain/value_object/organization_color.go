package value_object

type OrganizationColor struct {
	value int
}

func NewOrganizationColor(value int) (OrganizationColor, error) {
	return OrganizationColor{value: value}, nil
}

func (c OrganizationColor) Value() int {
	return c.value
}

func (c OrganizationColor) Equal(other OrganizationColor) bool {
	return c.value == other.value
}
