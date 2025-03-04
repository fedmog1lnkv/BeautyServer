package entity

import (
	"beauty-server/internal/domain/enum"
	"beauty-server/internal/domain/errors"
	"beauty-server/internal/domain/value_object"
	"github.com/google/uuid"
)

type Staff struct {
	Id             uuid.UUID
	OrganizationId uuid.UUID
	Name           value_object.StaffName
	PhoneNumber    value_object.StaffPhoneNumber
	StaffRole      enum.StaffRole
	ServiceIds     []uuid.UUID
	TimeSlots      []TimeSlot
}

type StaffRepository interface {
	IsPhoneNumberUnique(phoneNumber value_object.StaffPhoneNumber) (bool, error)
}

type ServiceRepository interface {
	ExistsService(id uuid.UUID) bool
}

func NewStaff(id, organizationId uuid.UUID, name string, phoneNumber string, staffRepo StaffRepository, orgRepository OrganizationRepository) (*Staff, error) {
	staffPhone, err := value_object.NewStaffPhoneNumber(phoneNumber)
	if err != nil {
		return nil, err
	}
	isUniquePhone, err := staffRepo.IsPhoneNumberUnique(staffPhone)
	if err != nil {
		return nil, err
	}
	if !isUniquePhone {
		return nil, errors.NewErrStaffPhoneNumberIsNotUnique(phoneNumber)
	}

	organization, err := orgRepository.GetById(organizationId)
	if err != nil {
		return nil, err
	}
	if organization == nil {
		return nil, errors.NewErrErrOrganizationNotFound(organizationId)
	}

	staffName, err := value_object.NewStaffName(name)
	if err != nil {
		return nil, err
	}

	return &Staff{
		Id:             id,
		OrganizationId: organizationId,
		Name:           staffName,
		PhoneNumber:    staffPhone,
		StaffRole:      enum.Unknown,
		ServiceIds:     make([]uuid.UUID, 0),
		TimeSlots:      make([]TimeSlot, 0),
	}, nil
}

func (s *Staff) UpdateName(name string) error {
	staffName, err := value_object.NewStaffName(name)
	if err != nil {
		return err
	}

	if s.Name.Equal(staffName) {
		return nil
	}

	s.Name = staffName
	return nil
}

func (s *Staff) UpdateStaffRole(role enum.StaffRole) {
	if s.StaffRole != role {
		s.StaffRole = role
	}
}

func (s *Staff) AddServiceId(serviceId uuid.UUID, repository ServiceRepository) error {
	if repository.ExistsService(serviceId) {
		return errors.NewErrServiceNotFound(serviceId)
	}

	for _, id := range s.ServiceIds {
		if id == serviceId {
			return nil
		}
	}

	s.ServiceIds = append(s.ServiceIds, serviceId)
	return nil
}

func (s *Staff) RemoveServiceId(serviceId uuid.UUID) error {
	newServiceIds := make([]uuid.UUID, 0, len(s.ServiceIds))

	for _, id := range s.ServiceIds {
		if id != serviceId {
			newServiceIds = append(newServiceIds, id)
		}
	}

	s.ServiceIds = newServiceIds
	return nil
}

// TODO: timeslot different arguments
func (s *Staff) AddTimeSlot(timeSlot TimeSlot) error {
	for _, ts := range s.TimeSlots {
		if ts.Id == timeSlot.Id {
			return nil
		}
	}

	s.TimeSlots = append(s.TimeSlots, timeSlot)
	return nil
}

func (s *Staff) UpdateTimeSlot(updatedSlot TimeSlot) error {
	for i, ts := range s.TimeSlots {
		if ts.Id == updatedSlot.Id {
			s.TimeSlots[i] = updatedSlot
			return nil
		}
	}

	return errors.NewErrTimeSlotNotFound(updatedSlot.Id)
}
