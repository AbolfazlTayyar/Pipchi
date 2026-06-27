# Pipchi

Pipchi is a modular C# application (ASP.NET Core style) that provides a domain-driven foundation with a separate API, Core domain, and Infrastructure layers. It includes Docker support and compose definitions to run the system locally or in containers.

## Features
- Clean separation: Pipchi.Api (HTTP surface), Pipchi.Core (domain & business logic), Pipchi.Infrastructure (persistence, search, outbox).
- Dockerfile for the API and docker-compose configuration to run services together.
- Infrastructure includes an Elasticsearch area and an Outbox pattern implementation.
- Solution file and per-project csproj files to build with `dotnet`.

## Repository layout
Top-level entries of interest:
- Pipchi/  
  - src/
    - Pipchi.Api/              — API project (Program.cs, appsettings.json, Dockerfile, Endpoints/, MappingProfiles/, Models/)
    - Pipchi.Core/             — Domain layer (aggregates, handlers, events, value objects, interfaces)
    - Pipchi.Infrastructure/   — Infrastructure (Data, Elasticsearch, Outbox, implementations)
  - tests/                     — Tests for the Pipchi solution
- SharedKernel/
  - src/                       — Shared utilities and kernel code used across projects
- Pipchi.slnx                  — Solution file
- docker-compose.yml           — Compose configuration for local/dev orchestration
- docker-compose.override.yml  — Overrides for local development
- Directory.Build.props / Directory.Packages.props

How it fits together:
- Pipchi.Api is the HTTP surface and composes services from Pipchi.Core and Pipchi.Infrastructure. The API registers dependencies via ServiceCollectionExtensions and loads configuration from appsettings.json / appsettings.Development.json. Infrastructure provides persistence and search support (including Elasticsearch) and implements messaging/outbox concerns.

## Quick start

Prerequisites
- .NET SDK (version compatible with the projects in the solution; run `dotnet --version`)
- Docker & Docker Compose (to run containerized services)

Run locally with dotnet
1. Restore and build the solution:
   ```bash
   dotnet restore ./Pipchi.slnx
   dotnet build ./Pipchi.slnx -c Release
   ```
2. Run the API project:
   ```bash
   dotnet run --project ./Pipchi/src/Pipchi.Api --configuration Release
   ```
3. Open the API root (default Kestrel port or as configured in `launchSettings.json` / `appsettings.json`).

Run with Docker Compose
1. Build & start services:
   ```bash
   docker-compose up --build
   ```
2. To start in the background:
   ```bash
   docker-compose up -d --build
   ```
3. Use `docker-compose logs -f` to stream logs and `docker-compose down` to stop and remove containers.

Notes:
- The API has a Dockerfile at `Pipchi/src/Pipchi.Api/Dockerfile`.
- `docker-compose.yml` and `docker-compose.override.yml` live at the repository root to define multi-service setups for development.

## Configuration
- The API reads configuration from `Pipchi/src/Pipchi.Api/appsettings.json` and `appsettings.Development.json`.
- Examine `ServiceCollectionExtensions.cs` files in both `Pipchi.Api` and `Pipchi.Infrastructure` to see which services and options are registered.
- Infrastructure folders suggest integrations with Elasticsearch and an outbox/message broker. Check `Pipchi/src/Pipchi.Infrastructure/Elasticsearch` and `Pipchi/src/Pipchi.Infrastructure/Outbox` for concrete settings and keys to supply in configuration.

Common env vars (check appsettings files and ServiceCollectionExtensions):
- ASPNETCORE_ENVIRONMENT — sets the environment (Development/Production).
- Connection strings and service URLs (look for keys in appsettings.json).
- Any broker or Elasticsearch URIs required by the infrastructure components.

## Build & test
- Build solution:
  ```bash
  dotnet build ./Pipchi.slnx
  ```
- Run tests:
  ```bash
  dotnet test ./Pipchi.slnx
  ```
- Add `-v minimal` or other dotnet flags if you need additional logging.

## Development notes
- Global usings are present in each project (GlobalUsings.cs) to centralize common usings.
- API endpoints are organized under `Pipchi/src/Pipchi.Api/Endpoints`.
- Mapping profiles (AutoMapper or similar) live under `Pipchi/src/Pipchi.Api/MappingProfiles`.
- Use `ServiceCollectionExtensions.cs` in both Api and Infrastructure projects to register or inspect DI wiring.

## Contributing
1. Fork the repository.
2. Create a feature branch: `git checkout -b feat/your-feature`.
3. Build and test locally.
4. Open a pull request with a clear description and any relevant notes about migrations, configuration, or breaking changes.

Please include tests for new features and run existing tests before submitting the PR.

## Troubleshooting
- If the API cannot connect to a service (Elasticsearch, database, message broker), check the configuration keys in `appsettings.json` and the environment variables used by Docker Compose.
- For container logs, use `docker-compose logs <service>`.

## TODO / Roadmap (suggested)
- Add a root README (this file) to the repository root (if not present).
- Document configuration keys and example `.env` for local development.
- Provide healthchecks and readiness probes in docker-compose for dependent services.
- Add sample requests or a Postman/HTTP client collection for the main API endpoints.

## License
Specify a license for the project (e.g., MIT). If you have a LICENSE file, include its name here.

---

If you want, I can:
- Add this README.md to the repository as a commit, or
- Expand the README with automatically-generated endpoint documentation by scanning the `Pipchi.Api/Endpoints` folder, or
- Create an example `.env` and docker-compose override file for local development.
