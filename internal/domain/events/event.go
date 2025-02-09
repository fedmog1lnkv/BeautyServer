package events

// Event - интерфейс для всех событий
type Event interface {
	Type() string
}
