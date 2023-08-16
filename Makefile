DOCKER_REGISTRY := jejikeh

# identity service
apply-users:
	kubectl apply -f ./src/Versity.Users/Deploy

up-users:
	docker-compose up -d versity-users-service

delete-secrets-users:
	 -kubectl delete secrets secret-identity-appsettings

secrets-users:
	kubectl create secret generic secret-identity-appsettings --from-file="./src/Versity.Users/Deploy/Secrets/appsettings.Production.json"

.PHONY: all
users: build-users push-users delete-secrets-users secrets-users apply-users

# products service

delete-secrets-products:
	 -kubectl delete secrets secret-products-appsettings

secrets-products:
	kubectl create secret generic secret-products-appsettings --from-file="./src/Versity.Products/Deploy/Secrets/appsettings.Production.json"

apply-products:
	kubectl apply -f ./src/Versity.Products/Deploy

up-products:
	docker-compose up -d versity-products-service

.PHONY: all
products: build-products push-products delete-secrets-products secrets-products apply-products

# apigateway service
build-apigateway:
	docker build -t $(DOCKER_REGISTRY)/versity.apigateway ./src/Versity.ApiGateway/

push-apigateway:
	docker push $(DOCKER_REGISTRY)/versity.apigateway

apply-apigateway:z
	kubectl apply -f ./src/Versity.Apigateway/Deploy

up-apigateway:
	docker-compose up -d versity-apigateway-service

delete-secrets-api-gateway:
	 -kubectl delete secrets secret-api-gateway-appsettings

secrets-api-gateway:
	kubectl create secret generic secret-api-gateway-appsettings --from-file="./src/Versity.ApiGateway/Deploy/Secrets/appsettings.Production.json"

.PHONY: all
apigateway: build-apigateway push-apigateway delete-secrets-api-gateway secrets-api-gateway apply-apigateway

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

.PHONY: all
deploy: users products apigateway setup-projects-secrets

.PHONY: all
apply: apply-users apply-products apply-apigateway