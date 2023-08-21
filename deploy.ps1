$USER_PATH = "./src/Versity.Users"
$PRODUCTS_PATH = "./src/Versity.Products"
$SESSIONS_PATH = "./src/Versity.Sessions"
$APIGATEWAY_PATH = "./src/Versity.ApiGateway/Versity.ApiGateway"
$FRONTEND_PATH = "./src/versity-frontend-react"

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

./clean.ps1
KubernetesSetupSecretsAll
ApplyKubernetesServicesAll