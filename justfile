#!/usr/bin/env just --justfile
DOCKER_ACCOUNT := "jejikeh"
SERVICE_NAME := "versity"

setup-ssl:
    -kubectl delete secrets {{SERVICE_NAME}}-tls-secret

    openssl genrsa -out {{SERVICE_NAME}}.key 2048
    openssl req -x509 \
      -new -nodes  \
      -days 365 \
      -key {{SERVICE_NAME}}.key \
      -out {{SERVICE_NAME}}.crt \
      -subj "/CN=localhost"

    kubectl create secret tls {{SERVICE_NAME}}-tls-secret \
      --key {{SERVICE_NAME}}.key \
      --cert {{SERVICE_NAME}}.crt