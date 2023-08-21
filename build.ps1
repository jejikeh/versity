$DOCKER_REGISTRY = $args[0]

$USER_PATH = "./src/Versity.Users"
$PRODUCTS_PATH = "./src/Versity.Products"
$SESSIONS_PATH = "./src/Versity.Sessions"
$APIGATEWAY_PATH = "./src/Versity.ApiGateway/Versity.ApiGateway"
$FRONTEND_PATH = "./src/versity-frontend-react"

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

BuildAndDeployToDockerAll