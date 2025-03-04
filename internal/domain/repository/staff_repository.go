package repository

import (
	"beauty-server/internal/domain/entity"
	"beauty-server/internal/domain/value_object"
	"github.com/google/uuid"
)

type StaffRepository interface {
	Save(staff *entity.Staff) error
	Update(staff *entity.Staff) error
	Remove(staff *entity.Staff) error
	GetByOrganizationId(staffId uuid.UUID) ([]*entity.Staff, error)
	GetByPhoneNumber(phoneNumber value_object.StaffPhoneNumber) (*entity.Staff, error)
}
