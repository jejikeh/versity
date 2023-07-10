function New-Cert
{
    ubuntu run openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout localhost.key -out localhost.crt -config localhost.conf -subj /CN=localhost
    ubuntu run openssl pkcs12 -export -out localhost.pfx -inkey localhost.key -in localhost.crt -passout pass:localhost
}

function Add-Cert
{
    $dir = Get-Location
    $fileName = "localhost.crt"
    $file = "$dir\$fileName"
    $localhostCaCert = New-Object -TypeName "System.Security.Cryptography.X509Certificates.X509Certificate2" @($file)
    $storeName = [System.Security.Cryptography.X509Certificates.StoreName]::Root
    $storeLocation = [System.Security.Cryptography.X509Certificates.StoreLocation]::LocalMachine
    $store = New-Object System.Security.Cryptography.X509Certificates.X509Store($storeName, $storeLocation)
    $store.Open(([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite))
    try
    {
        $store.Add($localhostCaCert)
        Write-Output "$fileName is successfully added to the Trusted Root."
    }
    finally
    {
        $store.Close()
        $store.Dispose()
    }
}

function Move-Files
{
    $userProfile = $ENV:USERPROFILE
    $httpsDir = "$userProfile\.aspnet\https"

    if (!(Test-Path -Path $httpsDir))
    {
        New-Item -ItemType directory -Path $httpsDir
    }

    Move-Item "localhost.pfx" $httpsDir -Force
    Copy-Item "localhost.crt" ../src/Versity.ApiGateway/
    Copy-Item "localhost.crt" ../src/Versity.Users/
    Copy-Item "localhost.crt" ../src/Versity.Products/
    Copy-Item "localhost.crt" ../src/Versity.Sessions/
    Remove-Item "localhost.key"
    Remove-Item "localhost.crt"
}

function Delete-Files 
{
    Param($File)
    if (Test-Path $File) {
        Remove-Item $File
    }
}

New-Cert
Add-Cert
Delete-Files -File ../src/Versity.ApiGateway/localhost.crt
Delete-Files -File ../src/Versity.Users/localhost.crt
Delete-Files -File ../src/Versity.Products/localhost.crt
Delete-Files -File ../src/Versity.Sessions/localhost.crt
Move-Files