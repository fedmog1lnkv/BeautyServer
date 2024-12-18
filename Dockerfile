FROM golang:1.20 AS builder

WORKDIR /app

COPY . .

RUN go mod tidy

RUN go build -o main .

FROM alpine:latest

RUN apk --no-cache add ca-certificates

COPY --from=builder /app/main /app/main

EXPOSE 8080

CMD ["/app/main"]
