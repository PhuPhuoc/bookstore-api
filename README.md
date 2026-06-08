# BookStore API

> A hands-on **Clean Architecture + DDD** learning project built with **ASP.NET Core 8**.  
> Focuses on implementing real-world enterprise patterns in a structured, layered .NET solution.

**Stack:** ASP.NET 8 · EF Core 9 (PostgreSQL) · Redis · MediatR · FluentValidation · ErrorOr · Mapster · Serilog

---

## 📐 Patterns & Implementation

### 🧱 Clean Architecture

6 projects, dependency chỉ đi theo một hướng — từ ngoài vào trong:

```
Api → Application → Domain
Api → Infrastructure.* → Application → Domain
Contracts → Api (Contracts là POCO, kéo vào từ Api)
```

**Nguyên tắc:** Domain không biết gì về Infrastructure, Application không biết gì về EF Core hay Redis. Muốn đổi PostgreSQL sang SQL Server hay Redis sang Memcached, chỉ cần sửa Infrastructure, không động đến business logic.

Mỗi layer có một trách nhiệm duy nhất:

| Layer | Trách nhiệm | Biết gì? |
|---|---|---|
| **Domain** | Entities, Value Objects, Domain Errors, Repository interfaces | Không biết gì — zero dependency |
| **Application** | Use cases (Commands/Queries), Handlers, Validators, Pipeline behaviors | Chỉ biết Domain |
| **Infrastructure.Persistence** | EF Core DbContext, Repository implementations, Migrations | Biết Application + Domain |
| **Infrastructure.Caching** | Redis client (shell) | Biết Application + Domain |
| **Api** | Controllers, Middleware, Mapster mapping, DI wiring | Biết tất cả layers bên dưới + Contracts |
| **Contracts** | Request/Response DTOs | Không biết gì — plain POCOs |

**Bằng chứng trong code:**
- `Author.cs` trong Domain không có `[Table]`, `[Column]` — cấu hình EF nằm hết trong `AuthorConfiguration.cs` ở Persistence
- `IAuthorRepository` được định nghĩa ở Domain, implement ở Persistence — Domain không cần reference EF Core
- `IUnitOfWork` interface ở Application, implementation (`UnitOfWork`) ở Persistence

---

### 🧩 Domain-Driven Design (DDD)

| Concept | Implementation | Ý nghĩa |
|---|---|---|
| **Entity** | `Entity<TId>` — base class chứa `Id` và domain event collection | Mọi entity đều có identity phân biệt qua Id |
| **Aggregate Root** | `AggregateRoot<TId>` — kế thừa `Entity<TId>`, là marker class | Chỉ Aggregate Root mới được repository fetch/save. Đảm bảo consistency boundary |
| **Value Object** | `AuthorId(Guid Value)` — `record` type | Strongly-typed Id: `AuthorId` ≠ `BookId` ≠ `Guid`. Compiler bắt lỗi ngay từ lúc gõ |
| **Domain Errors** | `AuthorErrors.NotFound` — static class chứa `Error` objects | Lỗi mang ý nghĩa business, không phải exception kỹ thuật |
| **Repository Interface** | `IAuthorRepository` trong Domain | Interface ở Domain, implementation ở Infrastructure — đảo ngược dependency |

> **Điểm đáng chú ý:** `Author` dùng **factory method** `Author.Create(...)` thay vì `new Author(...)` constructor public. Constructor private `Author() { }` chỉ dành cho EF Core. Đây là pattern phổ biến trong DDD để kiểm soát cách entity được tạo.

---

### ⚡ CQRS với MediatR

Mỗi use case là một Command (ghi) hoặc Query (đọc) riêng biệt, mỗi cái có Handler riêng. Không có service class "phình to" làm đủ thứ.

**Phân loại bằng marker interfaces:**
- `ICommand<TResponse> : IRequest<ErrorOr<TResponse>>` — command (ghi, cần SaveChanges)
- `IQuery<TResponse> : IRequest<ErrorOr<TResponse>>` — query (đọc, không cần SaveChanges)

**Pipeline behaviors xử lý xuyên suốt mọi request:**

| Behavior | Phạm vi | Việc làm |
|---|---|---|
| **ValidationBehavior** | Mọi request | Chạy FluentValidation trước handler. Lỗi → 422, không chạy handler |
| **UnitOfWorkBehavior** | Chỉ `ICommand<T>` | Tự động `SaveChangesAsync` nếu handler thành công. Lỗi → không commit |

> **Ý nghĩa:** Controller không bao giờ gọi `SaveChanges`. Handler không bao giờ gọi `SaveChanges`. Việc commit DB hoàn toàn do pipeline behavior quản lý — đảm bảo tính atomic của mỗi command.

---

### 🎯 ErrorOr — Result Pattern

Thay vì throw exception cho các lỗi nghiệp vụ (not found, conflict, validation), handler return `ErrorOr<T>`. Base `ApiController` dùng `result.Match(success → ..., errors → Problem(errors))` để map Error type sang HTTP status:

| Domain Error | HTTP Status |
|---|---|
| `ErrorType.NotFound` | 404 |
| `ErrorType.Conflict` | 409 |
| `ErrorType.Validation` | 422 |
| khác | 500 |

Exception chỉ dành cho lỗi thực sự bất ngờ (null reference, DB timeout, ...). Những exception này được `ExceptionHandlingMiddleware` bắt, log qua Serilog, và trả về JSON 500.

---

### 🔧 Chi tiết kỹ thuật

**FluentValidation + Reflection-based Error Construction**  
`ValidationBehavior` dùng `IValidator<TRequest>` (nullable — nếu không có validator thì bỏ qua). Khi validation fail, errors được gom thành `List<Error>` và dùng reflection để gọi `ErrorOr<T>.From(List<Error>)` — tránh unsafe `(dynamic)` cast.

**Mapster**  
Scan toàn bộ Api assembly để tìm các class implement `IRegister` (vd: `AuthorMappingConfig`). Controller chỉ cần gọi `_mapper.Map<Dest>(src)` — mapping tập trung, không có `new Response { ... }` rải rác.

**EF Core Fluent API**  
`AuthorConfiguration.cs` chứa toàn bộ cấu hình: table name, column name, type conversion (`AuthorId → Guid`). Domain entity `Author.cs` không hề có data annotation — sạch sẽ, không phụ thuộc ORM.

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
| | `Microsoft.AspNetCore.Authentication.JwtBearer` | 8.x | JWT (chưa wire) |
| | `Microsoft.EntityFrameworkCore.Design` | 9.x | EF CLI design-time |

---

## 🛠 Makefile

### Scaffolding (`make init`)
Chạy lần đầu để tạo toàn bộ solution từ zero:
```
new-sln → new-projects → sln-add → ref → add-packages → restore
```
Mỗi bước đều có thể chạy riêng lẻ nếu cần custom. `ref` tự động thiết lập project references theo đúng graph Clean Architecture.

### Package management (`make add` / `make remove`)
```bash
make add pkg=Newtonsoft.Json to=Application ver=13.0.3
make remove pkg=Newtonsoft.Json to=Application
```
Tự động resolve đường dẫn `.csproj` theo tên layer. Hỗ trợ version pinning.

### Database migrations (`make migration` / `make db-update`)
Tạo migration, update database, rollback, drop — target đúng project Infrastructure.Persistence, startup-project trỏ về Api.

### Run / Watch
```bash
make run     # dotnet run với https profile
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

# Hoặc dùng Makefile:
make run
```

---

## 📋 API Status

| Method | Route | Status |
|---|---|---|
| `POST` | `/api/authors` | ✅ Implemented |
| `GET` | `/api/authors` | ⏳ Planned |
| `GET` | `/api/authors/{id}` | ⏳ Planned |
| `PATCH` | `/api/authors/{id}` | ⏳ Planned |
| `DELETE` | `/api/authors/{id}` | ⏳ Planned |

Hiện tại chỉ có **Author** aggregate. **Book** entity và các endpoint liên quan đang trong kế hoạch.

---

## 💡 Key Design Decisions

| Decision | Rationale |
|---|---|
| **ICommand / IQuery markers** | Cho phép pipeline behavior phân biệt command (cần SaveChanges) với query (read-only) ngay tại generic constraint — không cần runtime check |
| **UnitOfWorkBehavior qua MediatR pipeline** | Controller và handler không bao giờ gọi `SaveChanges` trực tiếp. Transaction boundary là toàn bộ command — một commit hoặc không gì cả |
| **IUnitOfWork ở Application layer** | Application định nghĩa contract, Persistence implement. Đúng Dependency Inversion Principle |
| **Separate Infrastructure projects** | `Persistence` và `Caching` độc lập, không chung một project "rác". Swap cái nào cũng không ảnh hưởng cái kia |
| **Fluent API over data annotations** | Domain entities thuần POCO — không biết EF Core là gì |
| **AuthorId là value object** | Compile-time type safety: `AuthorId` ≠ `BookId` ≠ plain `Guid` |
| **Reflection-based error construction** | An toàn hơn `(dynamic)` cast — gọi đúng `ErrorOr<T>.From()` với generic type chính xác |
| **Makefile với scaffolding** | Tự động hoá tạo project structure đúng Clean Architecture ngay từ đầu — không lo sai dependency direction |
