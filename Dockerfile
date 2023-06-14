FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Versity.Users/External/Presentation/Presentation.csproj", "src/Versity.Users/External/Presentation/"]
COPY ["src/Versity.Users/External/Infrastructure/Infrastructure.csproj", "src/Versity.Users/External/Infrastructure/"]
COPY ["src/Versity.Users/Core/Application/Application.csproj", "src/Versity.Users/Core/Application/"]
COPY ["src/Versity.Users/Core/Domain/Domain.csproj", "src/Versity.Users/Core/Domain/"]
RUN dotnet restore "src/Versity.Users/External/Presentation/Presentation.csproj"
COPY . .
WORKDIR "/src/src/Versity.Users/External/Presentation"
RUN dotnet build "Presentation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Presentation.dll"]
