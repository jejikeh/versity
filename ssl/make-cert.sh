#!/bin/bash

function New-Cert {
    openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout localhost.key -out localhost.crt -config localhost.conf -subj "/CN=localhost"
    openssl pkcs12 -export -out localhost.pfx -inkey localhost.key -in localhost.crt -passout pass:localhost
}

function Add-Cert {
    dir=$(pwd)
    file="localhost.crt"
    filePath="$dir/$file"
    sudo security add-trusted-cert -d -r trustRoot -k "/Library/Keychains/System.keychain" "$filePath"
    echo "$file is successfully added to the Trusted Root."
}

function Move-Files {
    userProfile="$HOME"
    httpsDir="$userProfile/.aspnet/https"

    if [ ! -d "$httpsDir" ]; then
        mkdir -p "$httpsDir"
    fi

    mv "localhost.pfx" "$httpsDir"
    cp "localhost.crt" "../src/Versity.ApiGateway/"
    cp "localhost.crt" "../src/Versity.Users/"
    cp "localhost.crt" "../src/Versity.Products/"
    cp "localhost.crt" "../src/Versity.Sessions/"
    rm "localhost.key"
    rm "localhost.crt"
}

function Delete-Files {
    if [ -f "$1" ]; then
        rm "$1"
    fi
}

New-Cert
Add-Cert
Delete-Files "../src/Versity.ApiGateway/localhost.crt"
Delete-Files "../src/Versity.Users/localhost.crt"
Delete-Files "../src/Versity.Products/localhost.crt"
Delete-Files "../src/Versity.Sessions/localhost.crt"
Move-Files
