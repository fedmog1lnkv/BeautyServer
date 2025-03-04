package repository

import (
	"beauty-server/internal/domain/entity"
	"beauty-server/internal/infrastructure/model"
	"fmt"
	"github.com/google/uuid"
	"gorm.io/gorm"
)

type StaffRepository struct {
	DB *gorm.DB
}

func NewStaffRepository(db *gorm.DB) *StaffRepository {
	return &StaffRepository{DB: db}
}

func (r *StaffRepository) Save(staff *entity.Staff) error {
	staffModel := model.FromDomainStaff(staff)

	if err := r.DB.Create(staffModel).Error; err != nil {
		return fmt.Errorf("failed to save staff: %v", err)
	}

	for _, timeSlot := range staff.TimeSlots {
		timeSlotModel := model.FromDomainTimeSlot(&timeSlot)
		if err := r.DB.Create(&timeSlotModel).Error; err != nil {
			return fmt.Errorf("failed to save time slot: %v", err)
		}
	}

	return nil
}

func (r *StaffRepository) Update(staff *entity.Staff) error {
	staffModel := model.FromDomainStaff(staff)

	if err := r.DB.Save(staffModel).Error; err != nil {
		return fmt.Errorf("failed to update staff: %v", err)
	}

	for _, timeSlot := range staff.TimeSlots {
		timeSlotModel := model.FromDomainTimeSlot(&timeSlot)
		if err := r.DB.Save(&timeSlotModel).Error; err != nil {
			return fmt.Errorf("failed to update time slot: %v", err)
		}
	}

	return nil
}

func (r *StaffRepository) Remove(staff *entity.Staff) error {
	staffModel := model.FromDomainStaff(staff)

	if err := r.DB.Delete(staffModel).Error; err != nil {
		return fmt.Errorf("failed to remove staff: %v", err)
	}

	if err := r.DB.Where("staff_id = ?", staff.Id).Delete(&model.TimeSlotModel{}).Error; err != nil {
		return fmt.Errorf("failed to remove time slots: %v", err)
	}

	return nil
}

func (r *StaffRepository) GetByOrganizationId(organizationId uuid.UUID) ([]*entity.Staff, error) {
	var staffModels []model.StaffModel
	err := r.DB.Preload("TimeSlots").Where("organization_id = ?", organizationId).Find(&staffModels).Error
	if err != nil {
		return nil, fmt.Errorf("error fetching staff by organization id: %v", err)
	}

	var staffList []*entity.Staff
	for _, staffModel := range staffModels {
		staff, err := staffModel.ToDomain()
		if err != nil {
			return nil, fmt.Errorf("error converting staff model to domain: %v", err)
		}
		staffList = append(staffList, staff)
	}

	return staffList, nil
}

func (r *StaffRepository) GetById(staffId uuid.UUID) (*entity.Staff, error) {
	var staffModel model.StaffModel
	err := r.DB.Preload("TimeSlots").First(&staffModel, "id = ?", staffId).Error
	if err != nil {
		return nil, fmt.Errorf("error fetching staff by id: %v", err)
	}

	staff, err := staffModel.ToDomain()
	if err != nil {
		return nil, fmt.Errorf("error converting staff model to domain: %v", err)
	}

	return staff, nil
}
