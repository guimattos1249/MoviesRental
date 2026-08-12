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

# Consumer
COPY ["src/Services/Consumer/MoviesRental.Consumer/MoviesRental.Consumer.csproj", \
      "src/Services/Consumer/MoviesRental.Consumer/"]

# Shared - Application
COPY ["src/Shared/Application/MoviesRental.Queries.Application/MoviesRental.Queries.Application.csproj", \
      "src/Shared/Application/MoviesRental.Queries.Application/"]

# Shared - Domain
COPY ["src/Shared/Domain/MoviesRental.Queries.Domain/MoviesRental.Queries.Domain.csproj", \
      "src/Shared/Domain/MoviesRental.Queries.Domain/"]

# Shared - Infrastructure
COPY ["src/Shared/Infrastructure/MoviesRental.Queries.Infrastructure/MoviesRental.Queries.Infrastructure.csproj", \
      "src/Shared/Infrastructure/MoviesRental.Queries.Infrastructure/"]

# Building Blocks
COPY ["src/BuildingBlocks/MoviesRental.Core/MoviesRental.Core.csproj", \
      "src/BuildingBlocks/MoviesRental.Core/"]

# Restore
RUN dotnet restore \
    "./src/Services/Consumer/MoviesRental.Consumer/MoviesRental.Consumer.csproj"

# Copia o restante do código
COPY . .

# Define o diretório do projeto
WORKDIR "/src/src/Services/Consumer/MoviesRental.Consumer"

# Build
RUN dotnet build \
    "./MoviesRental.Consumer.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build \
    --no-restore


# ============================
# Publish
# ============================
FROM build AS publish

ARG BUILD_CONFIGURATION=Release

RUN dotnet publish \
    "./MoviesRental.Consumer.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false


# ============================
# Final
# ============================
FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "MoviesRental.Consumer.dll"]