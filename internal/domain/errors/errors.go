package errors

type ErrorType string

const (
	ErrorTypeNotFound     ErrorType = "NotFound"
	ErrorTypeValidation   ErrorType = "Validation"
	ErrorTypeConflict     ErrorType = "Conflict"
	ErrorTypeUnauthorized ErrorType = "Unauthorized"
	ErrorTypeInternal     ErrorType = "Internal"
)

type CustomError struct {
	Type    ErrorType `json:"type"`
	Message string    `json:"message"`
	Code    int       `json:"-"`
}

func New(errorType ErrorType, message string, code int) *CustomError {
	return &CustomError{
		Type:    errorType,
		Message: message,
		Code:    code,
	}
}

func (e *CustomError) Error() string {
	return e.Message
}
