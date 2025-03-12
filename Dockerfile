# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working directory inside the container
WORKDIR /app

# Copy solution and restore dependencies
COPY ["Bohio.Api/Bohio.Api.csproj", "Bohio.Api/"]
COPY ["Bohio.Application/Bohio.Application.csproj", "Bohio.Application/"]
COPY ["Bohio.Core/Bohio.Core.csproj", "Bohio.Core/"]
COPY ["Bohio.Infrastructure/Bohio.Infrastructure.csproj", "Bohio.Infrastructure/"]

RUN dotnet restore "Bohio.Api/Bohio.Api.csproj"

# Copy the entire application source code
COPY . .

# Build and publish the application
RUN dotnet publish "Bohio.Api/Bohio.Api.csproj" -c Release -o /app/publish -r linux-x64 --self-contained false

# Use the official ASP.NET Core runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set working directory
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/publish .

# Expose the port Fly.io will route traffic to
EXPOSE 5000

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Command to run the application
ENTRYPOINT ["dotnet", "Bohio.Api.dll"]
