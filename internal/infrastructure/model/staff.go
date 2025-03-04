package model

import (
	"beauty-server/internal/domain/entity"
	"beauty-server/internal/domain/enum"
	"beauty-server/internal/domain/value_object"
	"github.com/google/uuid"
)

type StaffModel struct {
	ID             uuid.UUID       `gorm:"type:uuid;primaryKey"`
	OrganizationID uuid.UUID       `gorm:"type:uuid;not null"`
	Name           string          `gorm:"type:varchar(255);not null"`
	PhoneNumber    string          `gorm:"type:varchar(20);not null"`
	StaffRole      enum.StaffRole  `gorm:"type:staff_role;not null"`
	ServiceIds     []uuid.UUID     `gorm:"-"` // only for read TODO : test
	TimeSlots      []TimeSlotModel `gorm:"foreignKey:StaffID;references:ID"`

	Organization OrganizationModel `gorm:"foreignKey:OrganizationID;references:Id"`
}

func (StaffModel) TableName() string {
	return "staff"
}

func (m *StaffModel) ToDomain() (*entity.Staff, error) {
	name, err := value_object.NewStaffName(m.Name)
	if err != nil {
		return nil, err
	}

	phoneNumber, err := value_object.NewStaffPhoneNumber(m.PhoneNumber)
	if err != nil {
		return nil, err
	}

	var timeSlots []entity.TimeSlot
	for _, ts := range m.TimeSlots {
		timeSlot, err := ts.ToDomain()
		if err != nil {
			return nil, err
		}
		timeSlots = append(timeSlots, *timeSlot)
	}

	staff := &entity.Staff{
		Id:             m.ID,
		OrganizationId: m.OrganizationID,
		Name:           name,
		PhoneNumber:    phoneNumber,
		StaffRole:      m.StaffRole,
		ServiceIds:     m.ServiceIds,
		TimeSlots:      timeSlots,
	}
	return staff, nil
}

func FromDomainStaff(staff *entity.Staff) *StaffModel {
	name := staff.Name.Value()
	phoneNumber := staff.PhoneNumber.Value()

	var timeSlots []TimeSlotModel
	for _, ts := range staff.TimeSlots {
		timeSlotModel := FromDomainTimeSlot(&ts)
		timeSlots = append(timeSlots, *timeSlotModel)
	}

	return &StaffModel{
		ID:             staff.Id,
		OrganizationID: staff.OrganizationId,
		Name:           name,
		PhoneNumber:    phoneNumber,
		StaffRole:      staff.StaffRole,
		ServiceIds:     staff.ServiceIds,
		TimeSlots:      timeSlots,
	}
}
