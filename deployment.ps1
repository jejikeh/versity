$DOCKER_REGISTRY = $args[0]

$USER_PATH = "./src/Versity.Users"
$PRODUCTS_PATH = "./src/Versity.Products"
$SESSIONS_PATH = "./src/Versity.Sessions"
$APIGATEWAY_PATH = "./src/Versity.ApiGateway/Versity.ApiGateway"
$FRONTEND_PATH = "./src/versity-frontend-react"

function InitUserSecret {
    param (
        $PROJECT_PATH
    )

    dotnet user-secrets init --project $PROJECT_PATH
}

function InitUserSecretsAll {
    Write-Host Start Initializing user-secrets projects
    Write-Host
    
    InitUserSecret -PROJECT_PATH $USER_PATH/External/Presentation
    InitUserSecret -PROJECT_PATH $PRODUCTS_PATH/External/Presentation
    InitUserSecret -PROJECT_PATH $SESSIONS_PATH/External/Presentation
    InitUserSecret -PROJECT_PATH $APIGATEWAY_PATH
}

function SetupSecrets {
    param (
        $PROJECT_PATH
    )

    Write-Host Start Initializing user-secrets $PROJECT_PATH project
    Write-Host

    dotnet user-secrets set "Smtp:Password" "ebqatxmtxaurcdfu" --project $PROJECT_PATH
    dotnet user-secrets set "Jwt:Key" "865D92FD-B1C8-41A4-850F-409792C9B740" --project $PROJECT_PATH
    dotnet user-secrets set "Jwt:Audience" "versity.identity" --project $PROJECT_PATH
    dotnet user-secrets set "Jwt:Issuer" "versity.identity" --project $PROJECT_PATH
    dotnet user-secrets set "Admin:Password" "versity.Adm1n.dev-31_13%versity" --project $PROJECT_PATH
    dotnet user-secrets set "Admin:Email" "versity.identity.dev@gmail.com" --project $PROJECT_PATH
    dotnet user-secrets set "Admin:Id" "4e274126-1d8a-4dfd-a025-806987095809" --project $PROJECT_PATH
}

function SetupUserSecretsAll {
    SetupSecrets -PROJECT_PATH $USER_PATH/External/Presentation
    SetupSecrets -PROJECT_PATH $PRODUCTS_PATH/External/Presentation
    SetupSecrets -PROJECT_PATH $SESSIONS_PATH/External/Presentation
    SetupSecrets -PROJECT_PATH $APIGATEWAY_PATH
}

function BuildAndPushToDocker {
    param (
        $PROJECT_PATH,
        $IMAGE_NAME
    )

    Write-Host Start Start Build the $IMAGE_NAME Image
    Write-Host

    docker build -t $DOCKER_REGISTRY/$IMAGE_NAME $PROJECT_PATH
    docker push $DOCKER_REGISTRY/$IMAGE_NAME
}

function BuildAndDeployToDockerAll {
    BuildAndPushToDocker -PROJECT_PATH $USER_PATH -IMAGE_NAME versity.users
    BuildAndPushToDocker -PROJECT_PATH $PRODUCTS_PATH -IMAGE_NAME versity.products
    BuildAndPushToDocker -PROJECT_PATH $SESSIONS_PATH -IMAGE_NAME versity.sessions
    BuildAndPushToDocker -PROJECT_PATH $APIGATEWAY_PATH/../ -IMAGE_NAME versity.apigateway
    BuildAndPushToDocker -PROJECT_PATH $FRONTEND_PATH -IMAGE_NAME versity.frontend
}

function KubernetesSetupSecret {
    param (
        $PROJECT_PATH,
        $PROJECT_NAME
    )

    kubectl delete secrets $PROJECT_NAME-secret-appsettings
    kubectl create secret generic $PROJECT_NAME-secret-appsettings --from-file="$PROJECT_PATH"
}

function KubernetesSetupSecretsAll {
    KubernetesSetupSecret -PROJECT_PATH $USER_PATH/Deploy/Secrets/appsettings.Production.json -PROJECT_NAME versity-users
    KubernetesSetupSecret -PROJECT_PATH $PRODUCTS_PATH/Deploy/Secrets/appsettings.Production.json -PROJECT_NAME versity-products
    KubernetesSetupSecret -PROJECT_PATH $SESSIONS_PATH/Deploy/Secrets/appsettings.Production.json -PROJECT_NAME versity-sessions
    KubernetesSetupSecret -PROJECT_PATH $APIGATEWAY_PATH/../Deploy/Secrets/appsettings.Production.json -PROJECT_NAME versity-apigateway
}

function ApplyKubernetesServices {
    param (
        $PROJECT_PATH
    )

    kubectl apply -f $PROJECT_PATH
}

function ApplyKubernetesServicesAll {
    ApplyKubernetesServices -PROJECT_PATH $USER_PATH/Deploy/
    ApplyKubernetesServices -PROJECT_PATH $PRODUCTS_PATH/Deploy/
    ApplyKubernetesServices -PROJECT_PATH $SESSIONS_PATH/Deploy/
    ApplyKubernetesServices -PROJECT_PATH $APIGATEWAY_PATH/../Deploy/
}

InitUserSecretsAll
SetupUserSecretsAll
BuildAndDeployToDockerAll
KubernetesSetupSecretsAll
ApplyKubernetesServicesAll