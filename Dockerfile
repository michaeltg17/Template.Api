# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props ./
COPY Directory.Packages.props ./
COPY Template.slnx ./
COPY src/ src/
RUN dotnet publish src/Api/Api.csproj -c Release -o /app

# Stage 2: Build efbundle (self-contained migration runner)
FROM build AS efbundle
RUN dotnet tool install --tool-path /tools dotnet-ef
RUN /tools/dotnet-ef bundle --project src/Persistence/Persistence.csproj --startup-project src/Api/Api.csproj --context AppDbContext --output /efbundle --runtime linux-x64 --framework net10.0 --self-contained

# Stage 3: Runtime (alpine + docker-cli + keepassxc-cli for deployments)
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine
RUN apk add --no-cache docker-cli keepassxc
WORKDIR /app
COPY --from=build /app .
COPY --from=efbundle /efbundle/efbundle .
COPY src/Api/MigrationsEntrypoint.sh .
RUN chmod +x MigrationsEntrypoint.sh

ENV ASPNETCORE_URLS=http://0.0.0.0:8080

# Entrypoint script runs efbundle to migrate DB, then starts the app
ENTRYPOINT ["./MigrationsEntrypoint.sh"]