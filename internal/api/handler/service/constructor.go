package service

import "beauty-server/internal/application/service/service"

type ServiceHandler struct {
	serviceService *service.ServiceService
}

func NewServiceHandler(serviceService *service.ServiceService) *ServiceHandler {
	return &ServiceHandler{
		serviceService: serviceService,
	}
}
