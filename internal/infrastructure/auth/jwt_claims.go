package auth

import (
	"github.com/dgrijalva/jwt-go"
)

type Claims struct {
	UserID  string `json:"user_id"`
	IsAdmin bool   `json:"is_admin"`
	jwt.StandardClaims
}

// TODO : add from .env
var secretKey = []byte("your-very-secret-key")
