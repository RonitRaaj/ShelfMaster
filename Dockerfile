# 1. Use the official Microsoft .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything and restore/publish the WebAPI project
COPY . .
RUN dotnet restore "src/ShelfMaster.WebAPI/ShelfMaster.WebAPI.csproj"
RUN dotnet publish "src/ShelfMaster.WebAPI/ShelfMaster.WebAPI.csproj" -c Release -o /publish

# 2. Use the lightweight ASP.NET runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /publish .

# Expose the standard port Render expects (8080)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ShelfMaster.WebAPI.dll"]