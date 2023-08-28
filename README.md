# versity
versity - it is an online distribution platform for software solutions through subscription. The service allows users to access products through sessions. The administrator, in turn, can collect user logs, open sessions for them and create products.

This project was developed during an internship at Modsen. I was responsible for the full development of this project under the guidance of mentors.

# Architecture

This project was developed using clean microservices arhitecture with 3 services with following tech stack:
### Main Technologies
* ASP.Core used as main web fraemwork for .NET
* EF Core for ORM
* ASP.Core Identity for Authorization
* SignalR for real-time app functionality
* Ocelot for API Gateway

### Databases
* PostgreSQL as main db for production in 2 microservices
* MongoDB as db for one of there microservices
* SQLite for quick debug launch scenario 
* Redis Stack for Repsonse Caching

### Libraries 
* FluentValidation to validate requests and models
* ElasticSearch is used for structured loggging
* Serilog as log provider, integrate with ELS
* Hangfire for background proccesing jobs
* MediatR for implementing CQRS pattern
* Scrutor to implement decorator pattern
* MailKit for sending emails

### Testing
* FluentAssertions is used for more readable assertions in Unit Tests
* xUnit as main test fraemwork
* TestContainers for Integration Tests

### Microservices Communication
* gRPC for communication between services
* Confluent Kafka for message bus based communiaction
* Http for quick debug launch (instead of Confluent Kafka when launch outside docker)
* JWT Based auth with Identity ASP.Core + Refresh Tokens

### CI/CD
* Docker and Kubernetes for deploying
* Github Actions for CI/CD
* Google Cloud GKE for hosting

### Frontend
* ReactJS with Typescript as main Web Framemwork
* MaterialUI for design elements
* Axios as http client
* SignalR client for real time connection

### Tooling
* Powershell scripts for Windows
* Bash scripts for Linux and OSX
* Golang for GKE-deployment helping util

# Launch

To launch application in local environment, you need run these commands:

```bash
# Setup User Secrets inside Kubernetes and .NET
./setup.sh 

# Build And Push Docker Images
./build.sh yor_name

# Run Kubernetes services
./deploy.sh
```