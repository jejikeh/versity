DOCKER_REGISTRY := jejikeh

# identity service
build-users:
	docker build -t $(DOCKER_REGISTRY)/versity.users ./src/Versity.Users/

push-users:
	docker push $(DOCKER_REGISTRY)/versity.users

up-users:
	docker-compose up -d versity-users-service

.PHONY: all
users: build-users push-users up-users

# products service
build-products:
	docker build -t $(DOCKER_REGISTRY)/versity.products ./src/Versity.Products/

push-products:
	docker push $(DOCKER_REGISTRY)/versity.products

up-products:
	docker-compose up -d versity-products-service

.PHONY: all
products: build-products push-products up-products

# apigateway service
build-apigateway:
	docker build -t $(DOCKER_REGISTRY)/versity.apigateway ./src/Versity.ApiGateway/

push-apigateway:
	docker push $(DOCKER_REGISTRY)/versity.apigateway

up-apigateway:
	docker-compose up -d versity-apigateway-service

.PHONY: all
apigateway: build-apigateway push-apigateway up-apigateway

# sessions service
build-sessions:
	docker build -t $(DOCKER_REGISTRY)/versity.sessions ./src/Versity.Sessions/

push-sessions:
	docker push $(DOCKER_REGISTRY)/versity.sessions

up-sessions:
	docker-compose up -d versity-sessions-service

.PHONY: all
sessions: build-sessions push-sessions up-sessions

up:
	docker-compose up

.PHONY: all
all: build-users push-users build-products push-products build-apigateway push-apigateway build-sessions push-sessions up

.PHONY: all
build-all: build-users push-users build-products push-products build-apigateway push-apigateway build-sessions push-sessions