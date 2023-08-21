#!/bin/bash

DOCKER_REGISTRY=$1

USER_PATH="./src/Versity.Users"
PRODUCTS_PATH="./src/Versity.Products"
SESSIONS_PATH="./src/Versity.Sessions"
APIGATEWAY_PATH="./src/Versity.ApiGateway/Versity.ApiGateway"
FRONTEND_PATH="./src/versity-frontend-react"

function KubernetesSetupSecret {
    PROJECT_PATH=$1
    PROJECT_NAME=$2

    kubectl delete secrets "$PROJECT_NAME-secret-appsettings"
    kubectl create secret generic "$PROJECT_NAME-secret-appsettings" --from-file="$PROJECT_PATH"
}

function KubernetesSetupSecretsAll {
    KubernetesSetupSecret "$USER_PATH/Deploy/Secrets/appsettings.Production.json" "versity-users"
    KubernetesSetupSecret "$PRODUCTS_PATH/Deploy/Secrets/appsettings.Production.json" "versity-products"
    KubernetesSetupSecret "$SESSIONS_PATH/Deploy/Secrets/appsettings.Production.json" "versity-sessions"
    KubernetesSetupSecret "$APIGATEWAY_PATH/../Deploy/Secrets/appsettings.Production.json" "versity-apigateway"
}

function ApplyKubernetesServices {
    PROJECT_PATH=$1

    kubectl apply -f "$PROJECT_PATH"
}

function ApplyKubernetesServicesAll {
    ApplyKubernetesServices "$USER_PATH/Deploy/"
    ApplyKubernetesServices "$PRODUCTS_PATH/Deploy/"
    ApplyKubernetesServices "$SESSIONS_PATH/Deploy/"
    ApplyKubernetesServices "$APIGATEWAY_PATH/../Deploy/"
    # ApplyKubernetesServices "$FRONTEND_PATH/Deploy/"    
}

KubernetesSetupSecretsAll
ApplyKubernetesServicesAll

