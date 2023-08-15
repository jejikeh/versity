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

setup-jwt-issuer-secret-apigateway:
	dotnet user-secrets set "Jwt:Issuer" "versity.identity" --project ./src/Versity.Apigateway/Versity.Apigateway

setup-jwt-issuer-secret-sessions:
	dotnet user-secrets set "Jwt:Issuer" "versity.identity" --project ./src/Versity.Sessions/External/Presentation

setup-jwt-issuer-secret:
	dotnet user-secrets set "Jwt:Issuer" "versity.identity" --project ./src/Versity.Users/External/Presentation

setup-jwt-issuer-secret-products:
	dotnet user-secrets set "Jwt:Issuer" "versity.identity" --project ./src/Versity.Products/External/Presentation

setup-jwt-audience-secret:
	dotnet user-secrets set "Jwt:Audience" "versity.identity" --project ./src/Versity.Users/External/Presentation

setup-jwt-audience-secret-products:
	dotnet user-secrets set "Jwt:Audience" "versity.identity" --project ./src/Versity.Products/External/Presentation

setup-jwt-audience-secret-sessions:
	dotnet user-secrets set "Jwt:Audience" "versity.identity" --project ./src/Versity.Sessions/External/Presentation

setup-jwt-audience-secret-apigateway:
	dotnet user-secrets set "Jwt:Audience" "versity.identity" --project ./src/Versity.Apigateway/Versity.Apigateway

setup-jwt-key-secret:
	dotnet user-secrets set "Jwt:Key" "865D92FD-B1C8-41A4-850F-409792C9B740" --project ./src/Versity.Users/External/Presentation

setup-jwt-key-secret-products:
	dotnet user-secrets set "Jwt:Key" "865D92FD-B1C8-41A4-850F-409792C9B740" --project ./src/Versity.Products/External/Presentation

setup-jwt-key-secret-sessions:
	dotnet user-secrets set "Jwt:Key" "865D92FD-B1C8-41A4-850F-409792C9B740" --project ./src/Versity.Sessions/External/Presentation

setup-jwt-key-secret-apigateway:
	dotnet user-secrets set "Jwt:Key" "865D92FD-B1C8-41A4-850F-409792C9B740" --project ./src/Versity.Apigateway/Versity.Apigateway

setup-email-password-secret:
	dotnet user-secrets set "Smtp:Password" "ebqatxmtxaurcdfu" --project ./src/Versity.Users/External/Presentation

.PHONY: all
setup-projects-secrets: setup-project-secrets-users setup-project-secrets-products setup-project-secrets-sessions setup-admin-id-secret setup-admin-email-secret setup-admin-password-secret setup-jwt-issuer-secret setup-jwt-audience-secret setup-jwt-key-secret setup-email-password-secret setup-jwt-issuer-secret-products setup-jwt-audience-secret-products setup-jwt-key-secret-products setup-jwt-issuer-secret-apigateway setup-jwt-audience-secret-apigateway setup-jwt-key-secret-apigateway setup-jwt-issuer-secret-sessions setup-jwt-audience-secret-sessions setup-jwt-key-secret-sessions

# identity service
build-users:
	docker build -t $(DOCKER_REGISTRY)/versity.users ./src/Versity.Users/

push-users:
	docker push $(DOCKER_REGISTRY)/versity.users

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
build-products:
	docker build -t $(DOCKER_REGISTRY)/versity.products ./src/Versity.Products/

push-products:
	docker push $(DOCKER_REGISTRY)/versity.products

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

apply-apigateway:
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