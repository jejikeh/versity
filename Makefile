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

up:
	docker-compose up