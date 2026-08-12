# ============================
# Runtime
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

WORKDIR /app
EXPOSE 8080

# ============================
# Build
# ============================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

# API
COPY ["src/Services/Publisher/API/MoviesRental.API/MoviesRental.API.csproj", \
      "src/Services/Publisher/API/MoviesRental.API/"]

# Publisher
COPY ["src/Services/Publisher/Application/MoviesRental.Application/MoviesRental.Application.csproj", \
      "src/Services/Publisher/Application/MoviesRental.Application/"]

COPY ["src/Services/Publisher/Domain/MoviesRental.Domain/MoviesRental.Domain.csproj", \
      "src/Services/Publisher/Domain/MoviesRental.Domain/"]

COPY ["src/Services/Publisher/Infrastructure/MoviesRental.Infrastructure/MoviesRental.Infrastructure.csproj", \
      "src/Services/Publisher/Infrastructure/MoviesRental.Infrastructure/"]

# Shared Queries
COPY ["src/Shared/Application/MoviesRental.Queries.Application/MoviesRental.Queries.Application.csproj", \
      "src/Shared/Application/MoviesRental.Queries.Application/"]

COPY ["src/Shared/Domain/MoviesRental.Queries.Domain/MoviesRental.Queries.Domain.csproj", \
      "src/Shared/Domain/MoviesRental.Queries.Domain/"]

COPY ["src/Shared/Infrastructure/MoviesRental.Queries.Infrastructure/MoviesRental.Queries.Infrastructure.csproj", \
      "src/Shared/Infrastructure/MoviesRental.Queries.Infrastructure/"]

# Building Blocks
COPY ["src/BuildingBlocks/MoviesRental.Core/MoviesRental.Core.csproj", \
      "src/BuildingBlocks/MoviesRental.Core/"]

RUN dotnet restore \
    "./src/Services/Publisher/API/MoviesRental.API/MoviesRental.API.csproj"

COPY . .

WORKDIR "/src/src/Services/Publisher/API/MoviesRental.API"

RUN dotnet build \
    "./MoviesRental.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build

# ============================
# Publish
# ============================
FROM build AS publish

ARG BUILD_CONFIGURATION=Release

RUN dotnet publish \
    "./MoviesRental.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# ============================
# Final
# ============================
FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "MoviesRental.API.dll"]