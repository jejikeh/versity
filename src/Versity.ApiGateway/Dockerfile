FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /
COPY ["/Versity.ApiGateway/Versity.ApiGateway.csproj", "/Versity.ApiGateway/"]
RUN dotnet restore "Versity.ApiGateway/Versity.ApiGateway.csproj"
COPY . .
WORKDIR "/Versity.ApiGateway/"
RUN dotnet build "Versity.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Versity.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Versity.ApiGateway.dll"]
