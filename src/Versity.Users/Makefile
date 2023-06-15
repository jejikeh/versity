IMAGE_NAME := versity.users
DOCKER_REGISTRY := jejikeh

.PHONY: build push

build:
	docker build -t $(DOCKER_REGISTRY)/$(IMAGE_NAME) .

push:
	docker push $(DOCKER_REGISTRY)/$(IMAGE_NAME)

up:
	docker-compose up $(ARGS)