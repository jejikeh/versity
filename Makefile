DOCKER_REGISTRY := jejikeh

build-users:
	docker build -t $(DOCKER_REGISTRY)/versity.users ./src/Versity.Users/

push-users:
	docker push $(DOCKER_REGISTRY)/versity.users

build-products:
	docker build -t $(DOCKER_REGISTRY)/versity.products ./src/Versity.Products/

push-products:
	docker push $(DOCKER_REGISTRY)/versity.products

build-apigateway:
	docker build -t $(DOCKER_REGISTRY)/versity.apigateway ./src/Versity.ApiGateway/

push-apigateway:
	docker push $(DOCKER_REGISTRY)/versity.apigateway

build-sessions:
	docker build -t $(DOCKER_REGISTRY)/versity.sessions ./src/Versity.Sessions/

push-sessions:
	docker push $(DOCKER_REGISTRY)/versity.sessions

up:
	docker-compose up

.PHONY: all
all: build-users push-users build-products push-products build-apigateway push-apigateway build-sessions push-sessions up

.PHONY: all
build-all: build-users push-users build-products push-products build-apigateway push-apigateway build-sessions push-sessions