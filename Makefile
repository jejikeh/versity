DOCKER_REGISTRY := jejikeh


setup-project-secrets-users:
	dotnet user-secrets init --project ./src/Versity.Users/External/Presentation

setup-project-secrets-products:
	dotnet user-secrets init --project ./src/Versity.Products/External/Presentation

setup-project-secrets-sessions:
	dotnet user-secrets init --project ./src/Versity.Sessions/External/Presentation

setup-admin-id-secret:
	dotnet user-secrets set "Admin:Id" "4e274126-1d8a-4dfd-a025-806987095809" --project ./src/Versity.Users/External/Presentation

setup-admin-email-secret:
	dotnet user-secrets set "Admin:Email" "versity.identity.dev@gmail.com" --project ./src/Versity.Users/External/Presentation

setup-admin-password-secret:
	dotnet user-secrets set "Admin:Password" "versity.Adm1n.dev-31_13%versity" --project ./src/Versity.Users/External/Presentation

setup-jwt-issuer-secret:
	dotnet user-secrets set "Jwt:Issuer" "versity.identity" --project ./src/Versity.Users/External/Presentation

setup-jwt-audience-secret:
	dotnet user-secrets set "Jwt:Audience" "versity.identity" --project ./src/Versity.Users/External/Presentation

setup-jwt-key-secret:
	dotnet user-secrets set "Jwt:Key" "865D92FD-B1C8-41A4-850F-409792C9B740" --project ./src/Versity.Users/External/Presentation

.PHONY: all
setup-projects-secrets: setup-project-secrets-users setup-project-secrets-products setup-project-secrets-sessions setup-admin-id-secret setup-admin-email-secret setup-admin-password-secret setup-jwt-issuer-secret setup-jwt-audience-secret setup-jwt-key-secret

# identity service
build-users:
	docker build -t $(DOCKER_REGISTRY)/versity.users ./src/Versity.Users/

push-users:
	docker push $(DOCKER_REGISTRY)/versity.users

apply-users:
	kubectl apply -f ./src/Versity.Users/Deploy

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