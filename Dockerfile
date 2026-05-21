# 1. Use the lightweight Alpine .NET 10.0 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /app

# Install native compilation essentials required by Alpine
RUN apk add --no-cache clang icu-libs

# Copy the solution and restore dependencies
COPY . .
RUN dotnet restore "src/ShelfMaster.WebAPI/ShelfMaster.WebAPI.csproj"

# Build and publish a trimmed production release package
RUN dotnet publish "src/ShelfMaster.WebAPI/ShelfMaster.WebAPI.csproj" \
    -c Release \
    -o /publish \
    --no-restore

# 2. Use the minimal Alpine .NET 10.0 runtime image for execution
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app
COPY --from=build /publish .

# Configure globalization support for Alpine
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Bind to Render's expected port
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ShelfMaster.WebAPI.dll"]