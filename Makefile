SLN        = BookStore.sln
PREFIX     = BookStore
API_DIR    = api/$(PREFIX).Api
API_PROJ   = $(API_DIR)/$(PREFIX).Api.csproj
PERSIST_PROJ = src/$(PREFIX).Infrastructure.Persistence/$(PREFIX).Infrastructure.Persistence.csproj
CACHE_PROJ   = src/$(PREFIX).Infrastructure.Caching/$(PREFIX).Infrastructure.Caching.csproj

# Pin versions to .NET 8 compatible releases.
# Each Serilog sink has its own independent versioning.
# JwtBearer version must match the target framework major version (net8.0 -> 8.x).
# Npgsql.EF follows EF Core versioning (9.x = EF Core 9 + Postgres provider).
EF_VERSION              = 9.0.5
NPGSQL_VERSION          = 9.0.4
REDIS_VERSION           = 2.8.24
SERILOG_ASPNET_VERSION  = 9.0.0
SERILOG_CONSOLE_VERSION = 6.1.1
JWT_VERSION             = 8.0.22

.DEFAULT_GOAL := run

# --------------------------------------------------------------------------
# Project layout:
#   api/   -> BookStore.Api                        (Presentation layer)
#   src/   -> BookStore.Contracts                  (Request/Response DTOs)
#             BookStore.Application                (Use Cases, CQRS, Validation)
#             BookStore.Domain                     (Entities, Aggregates, Interfaces)
#             BookStore.Infrastructure.Persistence (EF Core, Repositories, Migrations)
#             BookStore.Infrastructure.Caching     (Redis cache service)
# --------------------------------------------------------------------------

# --------------------------------------------------------------------------
# Solution & Project scaffolding
# Run once: make init
# --------------------------------------------------------------------------

.PHONY: new-sln
new-sln:
	dotnet new sln --name $(PREFIX) --output .
	@echo "Created solution $(SLN)"

.PHONY: new-projects
new-projects:
	dotnet new webapi   -n $(PREFIX).Api                        -o api/$(PREFIX).Api                        --no-openapi
	dotnet new classlib -n $(PREFIX).Contracts                  -o src/$(PREFIX).Contracts
	dotnet new classlib -n $(PREFIX).Application                -o src/$(PREFIX).Application
	dotnet new classlib -n $(PREFIX).Domain                     -o src/$(PREFIX).Domain
	dotnet new classlib -n $(PREFIX).Infrastructure.Persistence -o src/$(PREFIX).Infrastructure.Persistence
	dotnet new classlib -n $(PREFIX).Infrastructure.Caching     -o src/$(PREFIX).Infrastructure.Caching
	@echo "All projects created"

.PHONY: sln-add
sln-add:
	dotnet sln $(SLN) add api/$(PREFIX).Api/$(PREFIX).Api.csproj
	dotnet sln $(SLN) add src/$(PREFIX).Contracts/$(PREFIX).Contracts.csproj
	dotnet sln $(SLN) add src/$(PREFIX).Application/$(PREFIX).Application.csproj
	dotnet sln $(SLN) add src/$(PREFIX).Domain/$(PREFIX).Domain.csproj
	dotnet sln $(SLN) add src/$(PREFIX).Infrastructure.Persistence/$(PREFIX).Infrastructure.Persistence.csproj
	dotnet sln $(SLN) add src/$(PREFIX).Infrastructure.Caching/$(PREFIX).Infrastructure.Caching.csproj
	@echo "All projects registered in $(SLN)"

# Project reference graph (Clean Architecture):
#
#   Domain  <--  Application  <--  Infrastructure.Persistence
#                    ^                      ^
#                    |         Infrastructure.Caching
#                    |                      ^
#   Contracts <---- Api -------------------/
#
# Contracts:              plain POCOs, no references.
# Domain:                 no references.
# Application:            references Domain only.
# Infrastructure.*:       reference Application + Domain.
# Api:                    references Application + both Infrastructures + Contracts.
.PHONY: ref
ref:
	@echo "Setting up project references..."
	dotnet add src/$(PREFIX).Application/$(PREFIX).Application.csproj \
	    reference src/$(PREFIX).Domain/$(PREFIX).Domain.csproj
	dotnet add src/$(PREFIX).Infrastructure.Persistence/$(PREFIX).Infrastructure.Persistence.csproj \
	    reference src/$(PREFIX).Application/$(PREFIX).Application.csproj
	dotnet add src/$(PREFIX).Infrastructure.Persistence/$(PREFIX).Infrastructure.Persistence.csproj \
	    reference src/$(PREFIX).Domain/$(PREFIX).Domain.csproj
	dotnet add src/$(PREFIX).Infrastructure.Caching/$(PREFIX).Infrastructure.Caching.csproj \
	    reference src/$(PREFIX).Application/$(PREFIX).Application.csproj
	dotnet add src/$(PREFIX).Infrastructure.Caching/$(PREFIX).Infrastructure.Caching.csproj \
	    reference src/$(PREFIX).Domain/$(PREFIX).Domain.csproj
	dotnet add api/$(PREFIX).Api/$(PREFIX).Api.csproj \
	    reference src/$(PREFIX).Application/$(PREFIX).Application.csproj
	dotnet add api/$(PREFIX).Api/$(PREFIX).Api.csproj \
	    reference src/$(PREFIX).Infrastructure.Persistence/$(PREFIX).Infrastructure.Persistence.csproj
	dotnet add api/$(PREFIX).Api/$(PREFIX).Api.csproj \
	    reference src/$(PREFIX).Infrastructure.Caching/$(PREFIX).Infrastructure.Caching.csproj
	dotnet add api/$(PREFIX).Api/$(PREFIX).Api.csproj \
	    reference src/$(PREFIX).Contracts/$(PREFIX).Contracts.csproj
	@echo "Done. Dependency graph:"
	@echo "  Domain <- Application <- Infrastructure.Persistence"
	@echo "                        <- Infrastructure.Caching"
	@echo "  Contracts <- Api <- Application"
	@echo "               Api <- Infrastructure.Persistence"
	@echo "               Api <- Infrastructure.Caching"

# Install default packages for each layer.
# All versions are pinned to be compatible with net8.0.
.PHONY: add-packages
add-packages:
	@echo "Installing Application packages..."
	dotnet add src/$(PREFIX).Application/$(PREFIX).Application.csproj package MediatR
	dotnet add src/$(PREFIX).Application/$(PREFIX).Application.csproj package FluentValidation.DependencyInjectionExtensions
	dotnet add src/$(PREFIX).Application/$(PREFIX).Application.csproj package ErrorOr
	dotnet add src/$(PREFIX).Application/$(PREFIX).Application.csproj package Mapster
	dotnet add src/$(PREFIX).Application/$(PREFIX).Application.csproj package Mapster.DependencyInjection
	@echo "Installing Infrastructure.Persistence packages..."
	dotnet add src/$(PREFIX).Infrastructure.Persistence/$(PREFIX).Infrastructure.Persistence.csproj package Microsoft.EntityFrameworkCore --version $(EF_VERSION)
	dotnet add src/$(PREFIX).Infrastructure.Persistence/$(PREFIX).Infrastructure.Persistence.csproj package Npgsql.EntityFrameworkCore.PostgreSQL --version $(NPGSQL_VERSION)
	dotnet add src/$(PREFIX).Infrastructure.Persistence/$(PREFIX).Infrastructure.Persistence.csproj package Microsoft.EntityFrameworkCore.Tools --version $(EF_VERSION)
	dotnet add src/$(PREFIX).Infrastructure.Persistence/$(PREFIX).Infrastructure.Persistence.csproj package Microsoft.Extensions.Configuration.Abstractions
	@echo "Installing Infrastructure.Caching packages..."
	dotnet add src/$(PREFIX).Infrastructure.Caching/$(PREFIX).Infrastructure.Caching.csproj package StackExchange.Redis --version $(REDIS_VERSION)
	dotnet add src/$(PREFIX).Infrastructure.Caching/$(PREFIX).Infrastructure.Caching.csproj package Microsoft.Extensions.Caching.StackExchangeRedis
	dotnet add src/$(PREFIX).Infrastructure.Caching/$(PREFIX).Infrastructure.Caching.csproj package Microsoft.Extensions.Configuration.Abstractions
	@echo "Installing Api packages..."
	dotnet add api/$(PREFIX).Api/$(PREFIX).Api.csproj package Swashbuckle.AspNetCore
	dotnet add api/$(PREFIX).Api/$(PREFIX).Api.csproj package Serilog.AspNetCore --version $(SERILOG_ASPNET_VERSION)
	dotnet add api/$(PREFIX).Api/$(PREFIX).Api.csproj package Serilog.Sinks.Console --version $(SERILOG_CONSOLE_VERSION)
	dotnet add api/$(PREFIX).Api/$(PREFIX).Api.csproj package Microsoft.AspNetCore.Authentication.JwtBearer --version $(JWT_VERSION)
	@echo "All packages installed"

# Full setup from scratch
.PHONY: init
init: new-sln new-projects sln-add ref add-packages restore
	@echo ""
	@echo "========================================"
	@echo "  BookStore solution is ready!"
	@echo "========================================"
	@echo "Next: make run"

# --------------------------------------------------------------------------
# Build & Run
# --------------------------------------------------------------------------

.PHONY: build
build:
	dotnet build

.PHONY: run
run:
	dotnet run --project $(API_PROJ) --launch-profile https

.PHONY: watch
watch:
	dotnet watch run --project $(API_PROJ) --launch-profile https

.PHONY: clean
clean:
	dotnet clean
	@powershell -Command "Get-ChildItem -Recurse -Include bin,obj | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue"
	@echo "Cleaned bin and obj directories"

.PHONY: restore
restore:
	dotnet restore

.PHONY: kill
kill:
	Get-Process dotnet | Stop-Process -Force

# --------------------------------------------------------------------------
# NuGet package management
# Usage: make add pkg=Newtonsoft.Json to=Application
#        make add pkg=SomePackage to=Application ver=1.2.3
#        make remove pkg=Newtonsoft.Json to=Application
# Layers:
#   Api | Contracts | Application | Domain
#   Infrastructure.Persistence | Infrastructure.Caching
# Note: Api lives under api/, all others live under src/
# --------------------------------------------------------------------------

.PHONY: add
add:
	@if [ "$(pkg)" = "" ]; then \
	    echo "Error: missing 'pkg'. Example: make add pkg=Newtonsoft.Json to=Application"; exit 1; fi
	@if [ "$(to)" = "" ]; then \
	    echo "Error: missing 'to' (Api | Contracts | Application | Domain | Infrastructure.Persistence | Infrastructure.Caching)"; exit 1; fi
	@if [ "$(to)" = "Api" ]; then \
	    dotnet add api/$(PREFIX).Api/$(PREFIX).Api.csproj package $(pkg) $(if $(ver),--version $(ver),); \
	else \
	    dotnet add src/$(PREFIX).$(to)/$(PREFIX).$(to).csproj package $(pkg) $(if $(ver),--version $(ver),); \
	fi

.PHONY: remove
remove:
	@if [ "$(pkg)" = "" ]; then \
	    echo "Error: missing 'pkg'. Example: make remove pkg=Newtonsoft.Json to=Application"; exit 1; fi
	@if [ "$(to)" = "" ]; then \
	    echo "Error: missing 'to' (Api | Contracts | Application | Domain | Infrastructure.Persistence | Infrastructure.Caching)"; exit 1; fi
	@if [ "$(to)" = "Api" ]; then \
	    dotnet remove api/$(PREFIX).Api/$(PREFIX).Api.csproj package $(pkg); \
	else \
	    dotnet remove src/$(PREFIX).$(to)/$(PREFIX).$(to).csproj package $(pkg); \
	fi

# --------------------------------------------------------------------------
# EF Core migrations (targets Infrastructure.Persistence)
# Usage: make migration name=InitialCreate
#        make db-update
#        make db-rollback name=<target migration>
#        make migration-remove    <- removes last unapplied migration
# --------------------------------------------------------------------------

.PHONY: migration
migration:
	@if [ "$(name)" = "" ]; then \
	    echo "Error: missing migration name. Example: make migration name=InitialCreate"; exit 1; fi
	dotnet ef migrations add $(name) \
	    --project $(PERSIST_PROJ) \
	    --startup-project $(API_PROJ)

.PHONY: db-update
db-update:
	dotnet ef database update \
	    --project $(PERSIST_PROJ) \
	    --startup-project $(API_PROJ)

.PHONY: db-rollback
db-rollback:
	@if [ "$(name)" = "" ]; then \
	    echo "Error: specify target migration. Example: make db-rollback name=InitialCreate"; exit 1; fi
	dotnet ef database update $(name) \
	    --project $(PERSIST_PROJ) \
	    --startup-project $(API_PROJ)

.PHONY: migration-remove
migration-remove:
	dotnet ef migrations remove \
	    --project $(PERSIST_PROJ) \
	    --startup-project $(API_PROJ)

.PHONY: db-drop
db-drop:
	dotnet ef database drop --force \
	    --project $(PERSIST_PROJ) \
	    --startup-project $(API_PROJ)

# --------------------------------------------------------------------------
# Utilities
# --------------------------------------------------------------------------

.PHONY: list-refs
list-refs:
	@echo "=== Project References ==="
	@echo "-- Api --"
	@dotnet list api/$(PREFIX).Api/$(PREFIX).Api.csproj reference
	@echo "-- Application --"
	@dotnet list src/$(PREFIX).Application/$(PREFIX).Application.csproj reference
	@echo "-- Infrastructure.Persistence --"
	@dotnet list src/$(PREFIX).Infrastructure.Persistence/$(PREFIX).Infrastructure.Persistence.csproj reference
	@echo "-- Infrastructure.Caching --"
	@dotnet list src/$(PREFIX).Infrastructure.Caching/$(PREFIX).Infrastructure.Caching.csproj reference
	@echo "-- Contracts (should be empty) --"
	@dotnet list src/$(PREFIX).Contracts/$(PREFIX).Contracts.csproj reference
	@echo "-- Domain (should be empty) --"
	@dotnet list src/$(PREFIX).Domain/$(PREFIX).Domain.csproj reference

.PHONY: list-pkgs
list-pkgs:
	dotnet list package

.PHONY: format
format:
	dotnet format

.PHONY: help
help:
	@echo ""
	@echo "================================================================"
	@echo "  BookStore - Makefile Commands"
	@echo "================================================================"
	@echo "  SCAFFOLDING (run once)"
	@echo "    make init                   Full setup from scratch"
	@echo "    -- or step by step --"
	@echo "    make new-sln                Create .sln file"
	@echo "    make new-projects           Create 6 projects (api/ + src/)"
	@echo "    make sln-add                Register projects in .sln"
	@echo "    make ref                    Set up project references"
	@echo "    make add-packages           Install default packages"
	@echo "----------------------------------------------------------------"
	@echo "  BUILD & RUN"
	@echo "    make run                    Run API (https profile)"
	@echo "    make watch                  Hot reload"
	@echo "    make build                  Build solution"
	@echo "    make clean                  Remove bin/obj"
	@echo "----------------------------------------------------------------"
	@echo "  PACKAGES"
	@echo "    make add pkg=X to=Y         Add package to layer Y"
	@echo "    make add pkg=X to=Y ver=Z   Add specific version"
	@echo "    make remove pkg=X to=Y      Remove package from layer Y"
	@echo "    make list-pkgs              List all packages"
	@echo "  Layers: Api | Contracts | Application | Domain"
	@echo "          Infrastructure.Persistence | Infrastructure.Caching"
	@echo "----------------------------------------------------------------"
	@echo "  DATABASE / EF CORE"
	@echo "    make migration name=X       Create a new migration"
	@echo "    make db-update              Apply pending migrations"
	@echo "    make db-rollback name=X     Roll back to migration X"
	@echo "    make migration-remove       Remove last unapplied migration"
	@echo "    make db-drop                Drop the database"
	@echo "----------------------------------------------------------------"
	@echo "  UTILITIES"
	@echo "    make list-refs              Show all project references"
	@echo "    make format                 Run dotnet format"
	@echo "    make help                   Show this menu"
	@echo "================================================================"
	@echo ""
