# Use the official .NET SDK image as a build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["GarageGenius.csproj", "."]
RUN dotnet restore "GarageGenius.csproj"

# Copy the remaining source code and build the application
COPY . .
WORKDIR /src
RUN dotnet build "GarageGenius.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
RUN dotnet publish "GarageGenius.csproj" -c $BUILD_CONFIGURATION -o /app/publish

# Use the official ASP.NET Core runtime image as a base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY --from=build /app/publish .

# Expose port 80
EXPOSE 80

# Start the application
ENTRYPOINT ["dotnet", "GarageGenius.dll"]
