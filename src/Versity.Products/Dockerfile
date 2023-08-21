FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /
COPY ["/External/Presentation/Presentation.csproj", "/External/Presentation/"]
COPY ["/External/Infrastructure/Infrastructure.csproj", "/External/Infrastructure/"]
COPY ["/Core/Application/Application.csproj", "/Core/Application/"]
COPY ["/Core/Domain/Domain.csproj", "/Core/Domain/"]
RUN dotnet restore "/External/Presentation/Presentation.csproj"
COPY . .
WORKDIR "/External/Presentation"
RUN dotnet build "Presentation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Presentation.dll"]
