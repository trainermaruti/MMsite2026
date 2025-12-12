FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MarutiTrainingPortal.csproj", "./"]
RUN dotnet restore "MarutiTrainingPortal.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "MarutiTrainingPortal.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MarutiTrainingPortal.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment (can be overridden)
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "MarutiTrainingPortal.dll"]

# USAGE:
# 
# Build:
#   docker build -t marutitraining:latest .
#
# Run (with environment variables):
#   docker run -d -p 5000:80 \
#     -e ASPNETCORE_Admin__Email="admin@marutitraining.com" \
#     -e ASPNETCORE_Admin__Password="SecurePassword123!" \
#     -e ConnectionStrings__DefaultConnection="Data Source=app.db" \
#     --name marutitraining \
#     marutitraining:latest
#
# Run with .env file:
#   docker run -d -p 5000:80 --env-file .env marutitraining:latest
#
# Docker Compose (see docker-compose.yml):
#   docker compose up -d
