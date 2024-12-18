package errors

import "net/http"

func NotFound(message string) *CustomError {
	return New(ErrorTypeNotFound, message, http.StatusNotFound)
}

func Validation(message string) *CustomError {
	return New(ErrorTypeValidation, message, http.StatusBadRequest)
}

func Conflict(message string) *CustomError {
	return New(ErrorTypeConflict, message, http.StatusConflict)
}

func Unauthorized(message string) *CustomError {
	return New(ErrorTypeUnauthorized, message, http.StatusUnauthorized)
}

func Internal(message string) *CustomError {
	return New(ErrorTypeInternal, message, http.StatusInternalServerError)
}
