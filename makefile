.PHONY: watch

watch:
	dotnet watch --project ./Bohio.Api/Bohio.Api.csproj run

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
	$(DOTNET) run --project ./Bohio.Api/Bohio.Api.csproj --configuration $(CONFIG)

# Apply database migrations (using Infrastructure for migrations)
migrate:
	$(DOTNET) ef database update --verbose --project ./Bohio.Infrastructure/Bohio.Infrastructure.csproj --startup-project ./Bohio.Api/Bohio.Api.csproj

# Create a new migration (use 'make migrate-new name=MigrationName')
migrate-new:
	$(DOTNET) ef migrations add $(name) --verbose --project ./Bohio.Infrastructure/Bohio.Infrastructure.csproj --startup-project ./Bohio.Api/Bohio.Api.csproj

# Run unit tests
test:
	$(DOTNET) test Bohio.Api.IntegrationTests/Bohio.Api.IntegrationTests.csproj
test-verbose:
	$(DOTNET) test Bohio.Api.IntegrationTests/Bohio.Api.IntegrationTests.csproj --logger "console;verbosity=detailed"

# Publish the project
publish:
	$(DOTNET) publish ./Bohio.Api/Bohio.Api.csproj -c $(CONFIG) -o $(BUILD_DIR)
