# BookStore API

> A hands-on **Clean Architecture + DDD** learning project built with **ASP.NET Core 8**.  
> Focuses on implementing real-world enterprise patterns in a structured, layered .NET solution.

**Stack:** ASP.NET 8 · EF Core 9 (PostgreSQL) · Redis · MediatR · FluentValidation · ErrorOr · Mapster · Serilog

---

## 📐 Patterns & Implementation

### 🧱 Clean Architecture

6 projects, dependencies flow inward only:

```
Api → Application → Domain
Api → Infrastructure.* → Application → Domain
Contracts → Api (Contracts are plain POCOs pulled in by Api)
```

**Principle:** Domain knows nothing about Infrastructure. Application knows nothing about EF Core or Redis. Swap PostgreSQL for SQL Server or Redis for Memcached by touching only Infrastructure — business logic stays untouched.

Each layer has a single responsibility:

| Layer | Responsibility | Knows About |
|---|---|---|
| **Domain** | Entities, Value Objects, Domain Errors, Repository interfaces | Nothing — zero dependency |
| **Application** | Use cases (Commands/Queries), Handlers, Validators, Pipeline behaviors | Domain only |
| **Infrastructure.Persistence** | EF Core DbContext, Repository implementations, Migrations | Application + Domain |
| **Infrastructure.Caching** | Redis client (shell) | Application + Domain |
| **Api** | Controllers, Middleware, Mapster mapping, DI wiring | All layers below + Contracts |
| **Contracts** | Request/Response DTOs | Nothing — plain POCOs |

**Evidence in code:**
- `Author.cs` in Domain has no `[Table]` or `[Column]` — EF configuration lives entirely in `AuthorConfiguration.cs` (Persistence)
- `IAuthorRepository` is defined in Domain, implemented in Persistence — Domain never references EF Core
- `IUnitOfWork` interface lives in Application, its implementation (`UnitOfWork`) lives in Persistence

---

### 🧩 Domain-Driven Design (DDD)

| Concept | Implementation | Purpose |
|---|---|---|
| **Entity** | `Entity<TId>` — base class with `Id` and domain event collection | Every entity has an identity distinguished by its Id |
| **Aggregate Root** | `AggregateRoot<TId>` — extends `Entity<TId>`, marker class | Only Aggregate Roots are fetched/saved via repositories. Enforces consistency boundary |
| **Value Object** | `AuthorId(Guid Value)` — `record` type | Strongly-typed Id: `AuthorId` ≠ `BookId` ≠ plain `Guid`. Compiler catches mistakes at compile time |
| **Domain Errors** | `AuthorErrors.NotFound` — static class holding `Error` objects | Errors carry business meaning, not technical exceptions |
| **Repository Interface** | `IAuthorRepository` in Domain | Interface in Domain, implementation in Infrastructure — dependency inversion |

> **Notable:** `Author` uses a **factory method** `Author.Create(...)` instead of a public `new Author(...)` constructor. The private `Author() { }` constructor exists solely for EF Core. This is a common DDD pattern to control how entities are created.

---

### ⚡ CQRS with MediatR

Each use case is a separate Command (write) or Query (read), each with its own Handler. No bloated service classes.

**Classification via marker interfaces:**
- `ICommand<TResponse> : IRequest<ErrorOr<TResponse>>` — command (write, needs SaveChanges)
- `IQuery<TResponse> : IRequest<ErrorOr<TResponse>>` — query (read, no SaveChanges)

**Pipeline behaviors applied to every request:**

| Behavior | Scope | Responsibility |
|---|---|---|
| **ValidationBehavior** | All requests | Runs FluentValidation before the handler. On failure → returns 422, handler never executes |
| **UnitOfWorkBehavior** | Only `ICommand<T>` | Auto-calls `SaveChangesAsync` after successful handler. On error → nothing is committed |

> **Why this matters:** Controllers never call `SaveChanges`. Handlers never call `SaveChanges`. Database commits are managed entirely by pipeline behaviors — guaranteeing atomicity for every command.

---

### 🎯 ErrorOr — Result Pattern

Instead of throwing exceptions for business logic failures (not found, conflict, validation), handlers return `ErrorOr<T>`. The base `ApiController` uses `result.Match(success → ..., errors → Problem(errors))` to map Error types to HTTP status codes:

| Domain Error | HTTP Status |
|---|---|
| `ErrorType.NotFound` | 404 |
| `ErrorType.Conflict` | 409 |
| `ErrorType.Validation` | 422 |
| other | 500 |

Exceptions are reserved for truly unexpected failures (null reference, DB timeout, ...). These are caught by `ExceptionHandlingMiddleware`, logged via Serilog, and returned as JSON 500.

---

### 🔧 Technical Details

**FluentValidation + Reflection-based Error Construction**  
`ValidationBehavior` uses `IValidator<TRequest>` (nullable — skips if no validator is registered). On validation failure, errors are collected into `List<Error>` and converted to the correct `ErrorOr<T>` type via reflection (`ErrorOr<T>.From(List<Error>)`) — avoiding an unsafe `(dynamic)` cast.

**Mapster**  
Scans the Api assembly for classes implementing `IRegister` (e.g. `AuthorMappingConfig`). Controllers call `_mapper.Map<Dest>(src)` — centralized mapping, no scattered `new Response { ... }` assignments.

**EF Core Fluent API**  
`AuthorConfiguration.cs` contains all configuration: table name, column names, type conversion (`AuthorId → Guid`). The Domain entity `Author.cs` has zero data annotations — clean and ORM-agnostic.

---

## 📦 Package Reference

| Project | Package | Version | Purpose |
|---|---|---|---|
| **Domain** | `ErrorOr` | 2.x | Result pattern |
| **Application** | `MediatR` | 14.x | CQRS dispatcher |
| | `FluentValidation.DependencyInjectionExtensions` | 12.x | Auto-register validators |
| | `Mapster` + `Mapster.DependencyInjection` | 10.x | Object mapping |
| **Infrastructure.Persistence** | `Microsoft.EntityFrameworkCore` | 9.x | ORM |
| | `Npgsql.EntityFrameworkCore.PostgreSQL` | 9.x | PostgreSQL provider |
| | `Microsoft.EntityFrameworkCore.Tools` | 9.x | EF CLI |
| | `Microsoft.Extensions.Configuration.Abstractions` | 10.x | Config binding |
| **Infrastructure.Caching** | `StackExchange.Redis` | 2.x | Redis client |
| | `Microsoft.Extensions.Caching.StackExchangeRedis` | 10.x | Distributed cache |
| | `Microsoft.Extensions.Configuration.Abstractions` | 10.x | Config binding |
| **Api** | `Swashbuckle.AspNetCore` | 10.x | Swagger |
| | `Serilog.AspNetCore` | 9.x | Request logging |
| | `Serilog.Sinks.Console` | 6.x | Console output |
| | `Microsoft.AspNetCore.Authentication.JwtBearer` | 8.x | JWT (not yet wired) |
| | `Microsoft.EntityFrameworkCore.Design` | 9.x | EF CLI design-time |

---

## 🛠 Makefile

### Scaffolding (`make init`)
Run once to create the entire solution from scratch:
```
new-sln → new-projects → sln-add → ref → add-packages → restore
```
Each step can run individually. `ref` automatically sets up project references following the Clean Architecture dependency graph.

### Package management (`make add` / `make remove`)
```bash
make add pkg=Newtonsoft.Json to=Application ver=13.0.3
make remove pkg=Newtonsoft.Json to=Application
```
Auto-resolves `.csproj` paths by layer name. Supports version pinning.

### Database migrations (`make migration` / `make db-update`)
Create migrations, update the database, rollback, drop — all target the correct Infrastructure.Persistence project with `--startup-project` pointing to Api.

### Run / Watch
```bash
make run     # dotnet run with https profile
make watch   # hot reload
```

---

## 🚀 Getting Started

```bash
# Prerequisites: Docker, .NET 8 SDK

# 1. Start PostgreSQL + Redis
docker compose up -d

# 2. Apply migrations
dotnet ef database update \
    --project src/BookStore.Infrastructure.Persistence \
    --startup-project api/BookStore.Api

# 3. Run
dotnet run --project api/BookStore.Api
# → Swagger: https://localhost:44399/swagger

# Or using Makefile:
make run
```

---

## 💡 Key Design Decisions

| Decision | Rationale |
|---|---|
| **ICommand / IQuery markers** | Lets pipeline behaviors distinguish commands (need SaveChanges) from queries (read-only) at the generic constraint level — no fragile runtime checks |
| **UnitOfWorkBehavior via MediatR pipeline** | Controllers and handlers never call `SaveChanges` directly. Transaction boundary is the entire command — one commit or nothing |
| **IUnitOfWork in Application layer** | Application defines the contract, Persistence provides the implementation. Follows Dependency Inversion Principle |
| **Separate Infrastructure projects** | `Persistence` and `Caching` evolve independently. No monolithic `Infrastructure` project with unrelated dependencies |
| **Fluent API over data annotations** | Domain entities stay POCO — zero awareness of EF, column names, or DB constraints |
| **AuthorId as value object** | Compile-time type safety: `AuthorId` ≠ `BookId` ≠ plain `Guid` |
| **Reflection-based error construction** | Safer than `(dynamic)` cast — calls `ErrorOr<T>.From()` with the correct generic type |
| **Makefile with scaffolding** | Automates project structure creation following Clean Architecture from the start — no risk of incorrect dependency direction |
