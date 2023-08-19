#!/bin/bash

DOCKER_REGISTRY=$1

USER_PATH="./src/Versity.Users"
PRODUCTS_PATH="./src/Versity.Products"
SESSIONS_PATH="./src/Versity.Sessions"
APIGATEWAY_PATH="./src/Versity.ApiGateway/Versity.ApiGateway"
FRONTEND_PATH="./src/versity-frontend-react"

function BuildAndPushToDocker {
    PROJECT_PATH=$1
    IMAGE_NAME=$2

    echo "Start Build the $IMAGE_NAME Image"
    echo

    docker buildx build --platform linux/amd64 -t "$DOCKER_REGISTRY/$IMAGE_NAME" "$PROJECT_PATH"
    docker push "$DOCKER_REGISTRY/$IMAGE_NAME"
}

function BuildAndDeployToDockerAll {
    BuildAndPushToDocker "$USER_PATH" "versity.users"
    BuildAndPushToDocker "$PRODUCTS_PATH" "versity.products"
    BuildAndPushToDocker "$SESSIONS_PATH" "versity.sessions"
    BuildAndPushToDocker "$APIGATEWAY_PATH/../" "versity.apigateway"
    BuildAndPushToDocker "$FRONTEND_PATH" "versity.frontend"    
}

BuildAndDeployToDockerAll
