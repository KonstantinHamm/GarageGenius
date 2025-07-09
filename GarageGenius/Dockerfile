# Use the official .NET SDK image as a build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
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
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# Copy the published application
COPY --from=build /app/publish .

# Create directory for certificates and set permissions
RUN mkdir -p /https && \
    chmod 755 /https

# Expose ports
EXPOSE 80
EXPOSE 443

# Set environment variables for HTTPS (these will be overridden by docker-compose)
ENV ASPNETCORE_URLS="https://+:443;http://+:80"

# Set the entry point and command
ENTRYPOINT ["dotnet"]
CMD ["GarageGenius.dll"]