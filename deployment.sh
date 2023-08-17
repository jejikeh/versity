#!/bin/bash

DOCKER_REGISTRY=$1

USER_PATH="./src/Versity.Users"
PRODUCTS_PATH="./src/Versity.Products"
SESSIONS_PATH="./src/Versity.Sessions"
APIGATEWAY_PATH="./src/Versity.ApiGateway/Versity.ApiGateway"
FRONTEND_PATH="./src/versity-frontend-react"

function InitUserSecret {
    PROJECT_PATH=$1

    dotnet user-secrets init --project $PROJECT_PATH
}

function InitUserSecretsAll {
    echo "Start Initializing user-secrets projects"
    echo

    InitUserSecret "$USER_PATH/External/Presentation"
    InitUserSecret "$PRODUCTS_PATH/External/Presentation"
    InitUserSecret "$SESSIONS_PATH/External/Presentation"
    InitUserSecret "$APIGATEWAY_PATH"
}

function SetupSecrets {
    PROJECT_PATH=$1

    echo "Start Initializing user-secrets $PROJECT_PATH project"
    echo

    dotnet user-secrets set "Smtp:Password" "ebqatxmtxaurcdfu" --project $PROJECT_PATH
    dotnet user-secrets set "Jwt:Key" "865D92FD-B1C8-41A4-850F-409792C9B740" --project $PROJECT_PATH
    dotnet user-secrets set "Jwt:Audience" "versity.identity" --project $PROJECT_PATH
    dotnet user-secrets set "Jwt:Issuer" "versity.identity" --project $PROJECT_PATH
    dotnet user-secrets set "Admin:Password" "versity.Adm1n.dev-31_13%versity" --project $PROJECT_PATH
    dotnet user-secrets set "Admin:Email" "versity.identity.dev@gmail.com" --project $PROJECT_PATH
    dotnet user-secrets set "Admin:Id" "4e274126-1d8a-4dfd-a025-806987095809" --project $PROJECT_PATH
}

function SetupUserSecretsAll {
    SetupSecrets "$USER_PATH/External/Presentation"
    SetupSecrets "$PRODUCTS_PATH/External/Presentation"
    SetupSecrets "$SESSIONS_PATH/External/Presentation"
    SetupSecrets "$APIGATEWAY_PATH"
}

function BuildAndPushToDocker {
    PROJECT_PATH=$1
    IMAGE_NAME=$2

    echo "Start Build the $IMAGE_NAME Image"
    echo

    docker build -t "$DOCKER_REGISTRY/$IMAGE_NAME" "$PROJECT_PATH"
    docker push "$DOCKER_REGISTRY/$IMAGE_NAME"
}

function BuildAndDeployToDockerAll {
    BuildAndPushToDocker "$USER_PATH" "versity.users"
    BuildAndPushToDocker "$PRODUCTS_PATH" "versity.products"
    BuildAndPushToDocker "$SESSIONS_PATH" "versity.sessions"
    BuildAndPushToDocker "$APIGATEWAY_PATH/../" "versity.apigateway"
    BuildAndPushToDocker "$FRONTEND_PATH" "versity.frontend"    
}

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
    ApplyKubernetesServices "$FRONTEND_PATH/Deploy/"    
}

InitUserSecretsAll
SetupUserSecretsAll
BuildAndDeployToDockerAll
KubernetesSetupSecretsAll
ApplyKubernetesServicesAll
