.PHONY: watch

watch:
	dotnet watch --project ./Homespirations.Api/Homespirations.Api.csproj run

PROJECT_NAME = homespiration
DOTNET = dotnet
BUILD_DIR = ./bin
CONFIG = Release

.PHONY: all build clean run migrate test publish

# Default build
all: build

# Clean the project
clean:
	$(DOTNET) clean
	rm -rf $(BUILD_DIR)

# Build the solution
build:
	$(DOTNET) build --configuration $(CONFIG)

# Run the application
run:
	$(DOTNET) run --project ./Homespirations.Api/Homespirations.Api.csproj --configuration $(CONFIG)

# Apply database migrations (using Infrastructure for migrations)
migrate:
	$(DOTNET) ef database update --verbose --project ./Homespirations.Infrastructure/Homespirations.Infrastructure.csproj --startup-project ./Homespirations.Api/Homespirations.Api.csproj

# Create a new migration (use 'make migrate-new name=MigrationName')
migrate-new:
	$(DOTNET) ef migrations add $(name) --project ./Homespirations.Infrastructure/Homespirations.Infrastructure.csproj --startup-project ./Homespirations.Api/Homespirations.Api.csproj

# Run unit tests
test:
	$(DOTNET) test --configuration $(CONFIG)

# Publish the project
publish:
	$(DOTNET) publish ./Homespirations.Api/Homespirations.Api.csproj -c $(CONFIG) -o $(BUILD_DIR)
