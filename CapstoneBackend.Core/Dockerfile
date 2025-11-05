FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CapstoneBackend.Core/CapstoneBackend.Core.csproj", "CapstoneBackend.Core/"]
RUN dotnet restore "CapstoneBackend.Core/CapstoneBackend.Core.csproj"
COPY . .
WORKDIR "/src/CapstoneBackend.Core"
RUN dotnet build "./CapstoneBackend.Core.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CapstoneBackend.Core.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CapstoneBackend.Core.dll"]
