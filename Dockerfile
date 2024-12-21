FROM golang:1.23 as builder
WORKDIR /build
COPY go.mod .
COPY go.sum .
RUN go mod download
COPY . .
RUN CGO_ENABLED=0 GOOS=linux go build -o /main ./cmd/main.go

RUN go install github.com/swaggo/swag/cmd/swag@latest
RUN swag init -g cmd/main.go

FROM alpine
ENV PATH="/go/bin:${PATH}"
COPY --from=builder main /bin/main
COPY --from=builder /build/docs /docs
RUN apk --no-cache add bash
ENTRYPOINT ["/bin/main"]
