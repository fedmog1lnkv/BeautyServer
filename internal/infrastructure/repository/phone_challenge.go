package repository

import (
	"beauty-server/internal/domain/entity"
	"beauty-server/internal/domain/repository"
	"beauty-server/internal/infrastructure/model"
	"bytes"
	"encoding/json"
	"fmt"
	"gorm.io/gorm"
	"net/http"
	"os"
	"time"
)

type PhoneChallengeRepository struct {
	DB *gorm.DB
}

func NewPhoneChallengeRepository(db *gorm.DB) repository.PhoneChallengeRepository {
	return &PhoneChallengeRepository{DB: db}
}

func (p PhoneChallengeRepository) Save(phoneChallenge *entity.PhoneChallenge) error {
	phoneChallengeModel := model.FromDomainPhoneChallenge(phoneChallenge)
	if err := p.DB.Create(phoneChallengeModel).Error; err != nil {
		return fmt.Errorf("failed to save phone challenge: %v", err)
	}
	return nil
}

func (p PhoneChallengeRepository) GetByPhoneNumber(phoneNumber string) (*entity.PhoneChallenge, error) {
	var phoneChallengeModel model.PhoneChallengeModel
	err := p.DB.Where("phone_number = ?", phoneNumber).First(&phoneChallengeModel).Error
	if err != nil {
		if err == gorm.ErrRecordNotFound {
			return nil, nil
		}
		return nil, fmt.Errorf("error fetching phone challenge by phone number: %v", err)
	}

	phoneChallenge, err := phoneChallengeModel.ToDomain()
	if err != nil {
		return nil, err
	}

	return phoneChallenge, nil
}

type AuthRequest struct {
	Phone string `json:"phone"`
}

type User struct {
	TelegramID int    `json:"telegramId"`
	Username   string `json:"username"`
	FirstName  string `json:"firstName"`
	LastName   string `json:"lastName"`
	Phone      string `json:"phone"`
}

type AuthResponse struct {
	IsSuccess     bool   `json:"isSuccess"`
	SystemMessage string `json:"systemMessage"`
	User          User   `json:"user"`
}

func (p PhoneChallengeRepository) SendAuthRequest(phoneNumber string) (bool, string, error) {
	client := &http.Client{Timeout: 60 * time.Second}
	requestBody, err := json.Marshal(AuthRequest{Phone: phoneNumber})
	if err != nil {
		return false, "", err
	}

	url := os.Getenv("TELEGRAM_2FA_SERVER")
	if url == "" {
		return false, "", fmt.Errorf("TELEGRAM_2FA_SERVER gde?")
	}

	token := os.Getenv("TELEGRAM_2FA_TOKEN")
	if token == "" {
		return false, "", fmt.Errorf("TELEGRAM_2FA_TOKEN gde?")
	}

	req, err := http.NewRequest("POST", url+"/api/challenge/auth", bytes.NewBuffer(requestBody))
	if err != nil {
		return false, "", err
	}

	req.Header.Set("Content-Type", "application/json")
	req.Header.Set("accessToken", token)

	resp, err := client.Do(req)
	if err != nil {
		return false, "", err
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusOK {
		return false, "", fmt.Errorf("error sending code")
	}

	var response AuthResponse
	if err := json.NewDecoder(resp.Body).Decode(&response); err != nil {
		return false, "", err
	}

	if !response.IsSuccess {
		return false, "", fmt.Errorf(response.SystemMessage)
	}

	userName := response.User.FirstName + " " + response.User.LastName
	return response.IsSuccess, userName, nil
}

func (p PhoneChallengeRepository) SendCode(phoneNumber, code string) error {
	url := os.Getenv("TELEGRAM_2FA_SERVER")
	if url == "" {
		return fmt.Errorf("TELEGRAM_2FA_SERVER gde?")
	}
	token := os.Getenv("TELEGRAM_2FA_TOKEN")
	if token == "" {
		return fmt.Errorf("TELEGRAM_2FA_TOKEN gde?")
	}

	message := fmt.Sprintf("Ваш код <b>%s</b> для входа в приложение. Не передавайте этот код посторонним.", code)

	data := map[string]string{
		"Phone":   phoneNumber,
		"Message": message,
	}
	jsonData, err := json.Marshal(data)
	if err != nil {
		return err
	}
	req, err := http.NewRequest("POST", url+"/api/sms/send", bytes.NewBuffer(jsonData))
	if err != nil {
		return err
	}

	req.Header.Set("Content-Type", "application/json")
	req.Header.Set("accessToken", token)

	client := &http.Client{}
	resp, err := client.Do(req)
	if err != nil {
		return err
	}
	defer resp.Body.Close()

	// Response
	if resp.StatusCode != http.StatusOK {
		return fmt.Errorf("Status code: %d", resp.StatusCode)
	}

	var response map[string]interface{}
	if err := json.NewDecoder(resp.Body).Decode(&response); err != nil {
		return err
	}

	if isSuccess, ok := response["isSuccess"].(bool); ok && isSuccess {
		return nil
	} else {
		return fmt.Errorf("error while sending message: %v", response["systemMessage"])
	}
}

func (p PhoneChallengeRepository) Remove(phoneChallenge *entity.PhoneChallenge) error {
	phoneChallengeModel := model.FromDomainPhoneChallenge(phoneChallenge)
	if err := p.DB.Delete(phoneChallengeModel).Error; err != nil {
		return fmt.Errorf("failed to remove phone challenge: %v", err)
	}
	return nil
}
