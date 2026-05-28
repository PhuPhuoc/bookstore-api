# BookStore

A self-learning project for **ASP.NET 8** built with **Clean Architecture** and **Domain-Driven Design (DDD)**.
The goal is to practice patterns and packages commonly used in real-world enterprise .NET projects.

**Tech stack:** ASP.NET 8 · EF Core 9 (PostgreSQL) · Redis · MediatR · FluentValidation · ErrorOr · Mapster · Serilog · JWT

---

## Project Structure

```
BookStore.sln
│
├── api/
│   └── BookStore.Api/                                <- Presentation layer (ASP.NET Web API)
│       ├── Controllers/
│       │   ├── ApiController.cs                      <- Base controller: unwraps ErrorOr<T> into HTTP responses
│       │   ├── BooksController.cs
│       │   └── AuthorsController.cs
│       ├── Middleware/
│       │   └── ErrorHandlingMiddleware.cs            <- Global unhandled exception handler
│       └── Program.cs                                <- DI wiring, middleware pipeline
│
├── src/
│   ├── BookStore.Contracts/                          <- HTTP layer DTOs (no business logic, no dependencies)
│   │   ├── Books/
│   │   │   ├── CreateBookRequest.cs
│   │   │   ├── UpdateBookRequest.cs
│   │   │   └── BookResponse.cs
│   │   └── Authors/
│   │       ├── CreateAuthorRequest.cs
│   │       └── AuthorResponse.cs
│   │
│   ├── BookStore.Domain/                             <- Core business logic (zero external dependencies)
│   │   ├── Common/
│   │   │   ├── Entity.cs                             <- Base class: typed Id, domain event collection
│   │   │   └── AggregateRoot.cs                      <- Marker class; all state changes go through the root
│   │   ├── Books/
│   │   │   ├── Book.cs                               <- Aggregate Root
│   │   │   ├── BookId.cs                             <- Value Object (strongly-typed Id wrapping Guid)
│   │   │   └── BookErrors.cs                         <- Domain-level errors for use with ErrorOr
│   │   ├── Authors/
│   │   │   ├── Author.cs
│   │   │   ├── AuthorId.cs
│   │   │   └── AuthorErrors.cs
│   │   └── Repositories/
│   │       ├── IBookRepository.cs                    <- Interface lives in Domain; implementation in Infrastructure
│   │       └── IAuthorRepository.cs
│   │
│   ├── BookStore.Application/                        <- Use cases: CQRS handlers, validation, mapping contracts
│   │   ├── Common/
│   │   │   ├── Behaviors/
│   │   │   │   └── ValidationBehavior.cs             <- MediatR pipeline: runs FluentValidation before every handler
│   │   │   └── Interfaces/
│   │   │       └── ICacheService.cs                  <- Cache abstraction defined here (not in Infrastructure)
│   │   ├── Books/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateBook/
│   │   │   │   │   ├── CreateBookCommand.cs
│   │   │   │   │   ├── CreateBookCommandHandler.cs
│   │   │   │   │   └── CreateBookCommandValidator.cs
│   │   │   │   ├── UpdateBook/
│   │   │   │   │   ├── UpdateBookCommand.cs
│   │   │   │   │   ├── UpdateBookCommandHandler.cs
│   │   │   │   │   └── UpdateBookCommandValidator.cs
│   │   │   │   └── DeleteBook/
│   │   │   │       ├── DeleteBookCommand.cs
│   │   │   │       └── DeleteBookCommandHandler.cs
│   │   │   └── Queries/
│   │   │       ├── GetBook/
│   │   │       │   ├── GetBookQuery.cs
│   │   │       │   └── GetBookQueryHandler.cs        <- may read from cache first, then DB
│   │   │       └── ListBooks/
│   │   │           ├── ListBooksQuery.cs
│   │   │           └── ListBooksQueryHandler.cs
│   │   └── DependencyInjection.cs
│   │
│   ├── BookStore.Infrastructure.Persistence/         <- EF Core (PostgreSQL write/read DB)
│   │   ├── AppDbContext.cs
│   │   ├── Configurations/
│   │   │   ├── BookConfiguration.cs                  <- EF Fluent API (keeps Domain free of data annotations)
│   │   │   └── AuthorConfiguration.cs
│   │   ├── Migrations/
│   │   ├── Repositories/
│   │   │   ├── BookRepository.cs                     <- Implements IBookRepository
│   │   │   └── AuthorRepository.cs
│   │   └── DependencyInjection.cs
│   │
│   └── BookStore.Infrastructure.Caching/             <- Redis cache
│       ├── RedisCacheService.cs                      <- Implements ICacheService from Application
│       ├── CacheKeys.cs                              <- Static constants for all cache key names
│       └── DependencyInjection.cs
│
└── Makefile
```

---

## Dependency Graph

```
┌─────────────┐
│  Contracts  │  plain POCOs, no references
└──────┬──────┘
       │
┌──────▼────────────────────────────────────────────────────┐
│                         Api                                │
│  receives HTTP Request -> maps to Command/Query            │
│  dispatches via MediatR -> maps result to HTTP Response    │
└──────┬───────────────────┬──────────────────┬─────────────┘
       │                   │                  │
┌──────▼──────┐  ┌─────────▼──────────┐  ┌───▼──────────────────┐
│ Application │  │ Infra.Persistence  │  │  Infra.Caching       │
│  Commands   │◄─│  BookRepository    │  │  RedisCacheService   │
│  Queries    │  │  AppDbContext      │  │  (implements         │
│  Validators │◄─│  (implements       │  │   ICacheService)     │
│  ICacheService│  IBookRepository)  │  └──────────────────────┘
└──────┬──────┘  └────────────────────┘
       │
┌──────▼──────┐
│   Domain    │  zero external dependencies
│  Aggregates │
│  Errors     │
│  Interfaces │
└─────────────┘
```

**Dependency rule:** arrows point inward only. Outer layers know inner layers; inner layers never reference outer layers.

Key point: `ICacheService` is defined in **Application**, not in Infrastructure. This means Application handlers can use caching without importing any Redis package — Infrastructure.Caching just provides the concrete implementation.

---

## Patterns & Why

### Clean Architecture
Business logic (Domain + Application) is completely isolated from infrastructure (EF Core, Redis) and the HTTP layer (API). You can swap PostgreSQL for another DB or Redis for Memcached without touching Domain or Application.

### Domain-Driven Design (DDD)

| Concept | Implementation |
|---|---|
| **Aggregate Root** | `Book`, `Author` — the single entry point for all state mutations |
| **Value Object** | `BookId(Guid Value)` — strongly-typed Id; prevents accidentally passing an `AuthorId` where a `BookId` is expected |
| **Domain Errors** | `BookErrors.NotFound`, `BookErrors.DuplicateIsbn` — errors carry business meaning |
| **Repository Interface** | Defined in Domain (`IBookRepository`), implemented in Infrastructure.Persistence (`BookRepository`) |

### CQRS with MediatR
Each use case (create, update, delete, get, list) is a separate Command or Query class with its own Handler. No fat service classes.

```
POST /books
  │
  ▼
BooksController.Create(CreateBookRequest)
  │  map via Mapster
  ▼
CreateBookCommand ──► MediatR pipeline
                           │
                     ValidationBehavior  (FluentValidation auto-runs here)
                           │
                     CreateBookCommandHandler
                           │
                     IBookRepository.AddAsync(book)
                           │
                     return ErrorOr<Book>
  │
  ▼
result.Match(
  book   => CreatedAtAction(nameof(Get), new { id = book.Id }, book.Adapt<BookResponse>()),
  errors => Problem(errors)
)
```

### ErrorOr — Result Pattern
Handlers return `ErrorOr<T>` instead of throwing exceptions. The base `ApiController` has a `Match` helper that maps `Error` types to the correct HTTP status codes (`404`, `409`, `422`, etc.). Exceptions are reserved for truly unexpected failures only.

```csharp
// In handler
public async Task<ErrorOr<Book>> Handle(CreateBookCommand cmd, CancellationToken ct)
{
    if (await _repo.ExistsByIsbn(cmd.Isbn, ct))
        return BookErrors.DuplicateIsbn;          // returns 409

    var book = Book.Create(cmd.Title, cmd.Isbn, cmd.AuthorId);
    await _repo.AddAsync(book, ct);
    return book;
}

// In base ApiController
protected IActionResult Problem(List<Error> errors)
{
    var first = errors.First();
    var statusCode = first.Type switch
    {
        ErrorType.NotFound   => StatusCodes.Status404NotFound,
        ErrorType.Conflict   => StatusCodes.Status409Conflict,
        ErrorType.Validation => StatusCodes.Status422UnprocessableEntity,
        _                    => StatusCodes.Status500InternalServerError,
    };
    return Problem(statusCode: statusCode, detail: first.Description);
}
```

### FluentValidation + MediatR Pipeline Behavior
`ValidationBehavior<TRequest, TResponse>` is registered as a `IPipelineBehavior`. It runs all validators for the incoming Command/Query automatically before the handler executes. Controllers never call `.Validate()` manually.

```csharp
// CreateBookCommandValidator.cs
public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Isbn).NotEmpty().Length(13);
        RuleFor(x => x.AuthorId).NotEmpty();
    }
}
```

### Mapster
Maps between layers (e.g. `Book` → `BookResponse`, `CreateBookRequest` → `CreateBookCommand`). Configured centrally in `DependencyInjection.cs` using `TypeAdapterConfig`. No scattered `new BookResponse { ... }` assignments.

### EF Core + Fluent API
Each aggregate has its own `IEntityTypeConfiguration<T>` file. Domain entities have no EF attributes (`[Column]`, `[MaxLength]`, etc.) — configuration is 100% in Infrastructure.Persistence.

### Redis Caching
`ICacheService` (defined in Application) provides a simple `GetAsync<T>` / `SetAsync<T>` / `RemoveAsync` contract. `RedisCacheService` (in Infrastructure.Caching) implements it using `StackExchange.Redis`. Query handlers that need caching inject `ICacheService` — they never reference Redis directly.

---

## Package Reference

| Project | Package | Version | Purpose |
|---|---|---|---|
| Application | `MediatR` | latest | CQRS dispatcher |
| Application | `FluentValidation.DependencyInjectionExtensions` | latest | Auto-register validators + pipeline |
| Application | `ErrorOr` | latest | Result pattern (replaces exceptions for domain errors) |
| Application | `Mapster` + `Mapster.DependencyInjection` | latest | Object mapping between layers |
| Infrastructure.Persistence | `Microsoft.EntityFrameworkCore` | 9.x | ORM core |
| Infrastructure.Persistence | `Npgsql.EntityFrameworkCore.PostgreSQL` | 9.x | PostgreSQL provider for EF Core |
| Infrastructure.Persistence | `Microsoft.EntityFrameworkCore.Tools` | 9.x | EF CLI (`dotnet ef migrations`) |
| Infrastructure.Persistence | `Microsoft.Extensions.Configuration.Abstractions` | latest | Read connection strings |
| Infrastructure.Caching | `StackExchange.Redis` | 2.x | Redis client |
| Infrastructure.Caching | `Microsoft.Extensions.Caching.StackExchangeRedis` | latest | IDistributedCache integration |
| Infrastructure.Caching | `Microsoft.Extensions.Configuration.Abstractions` | latest | Read Redis config |
| Api | `Swashbuckle.AspNetCore` | latest | Swagger / OpenAPI |
| Api | `Serilog.AspNetCore` | 9.x | Structured request logging |
| Api | `Serilog.Sinks.Console` | 6.x | Console sink (independent versioning from Serilog.AspNetCore) |
| Api | `Microsoft.AspNetCore.Authentication.JwtBearer` | 8.x | JWT auth (must match target framework major: net8.0 → 8.x) |

> **Version pinning notes:**
> - `EF Core` and `Npgsql.EF` → `9.x` (compatible with net8.0; `10.x` requires net10.0)
> - `Serilog.Sinks.Console` → `6.x` (has its own release cycle; latest is 6.1.1)
> - `JwtBearer` → `8.x` for net8.0 projects (version must match the TFM major version)

---

## Getting Started

```bash
# 1. Scaffold the full solution (run once in an empty folder)
make init

# 2. Configure connection strings in api/BookStore.Api/appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=bookstore;Username=postgres;Password=yourpassword",
    "Redis": "localhost:6379"
  }
}

# 3. Create the database schema
make migration name=InitialCreate
make db-update

# 4. Run
make run

# Hot reload during development
make watch
```

All available commands:

```bash
make help
```

---

## Adding / Removing Packages

```bash
make add pkg=Newtonsoft.Json to=Application
make add pkg=SomePackage to=Infrastructure.Persistence ver=9.0.5
make add pkg=Scalar.AspNetCore to=Api

make remove pkg=Newtonsoft.Json to=Application
```

Valid values for `to`:
`Api` | `Contracts` | `Application` | `Domain` | `Infrastructure.Persistence` | `Infrastructure.Caching`

---

## Key Design Decisions

| Decision | Reason |
|---|---|
| `ICacheService` defined in Application, not Infrastructure | Application handlers can depend on the cache abstraction without importing Redis. Follows Dependency Inversion. |
| Two separate Infrastructure projects | `Infrastructure.Persistence` and `Infrastructure.Caching` can evolve independently. Swap Redis for Memcached or switch DB provider without touching the other. |
| `BookId` as a Value Object | Prevents passing wrong Id types at compile time. `Book.Create()` takes a `BookId`, never a raw `Guid`. |
| No `Infrastructure` catch-all project | Keeping persistence and caching separate makes ownership clear and avoids one oversized `DependencyInjection.cs`. |
| EF Fluent API in Configurations/ | Domain entities stay free of ORM attributes. `Book.cs` knows nothing about databases. |
