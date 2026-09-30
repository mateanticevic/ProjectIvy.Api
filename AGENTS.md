# Project Ivy API

.NET 10 ASP.NET Core API. Solution is `ProjectIvy.sln`.

## Layout

- `src/ProjectIvy.Api` — HTTP controllers, `Startup` DI, MCP tools. No business logic or database access.
- `src/ProjectIvy.Business` — handlers and binding-to-entity maps.
- `src/ProjectIvy.Data` — EF Core `MainContext`, query filters, embedded SQL.
- `src/ProjectIvy.Model` — `Binding` (input), `View` (output), `Database` (entities).
- `src/ProjectIvy.Common` — shared helpers.
- `test/ProjectIvy.Data.Test` — xUnit tests for data helpers.

Copy `Ride` when adding a feature: `RideController`, `IRideHandler` / `RideHandler`, `RideBinding`, `Model/View/Ride/Ride`.

## Request flow

Controllers inherit `BaseController<T>`, use `[Route("[controller]")]`, take `ILogger<T>` plus one handler, and return the handler result.

```csharp
[HttpGet]
public async Task<IActionResult> Get([FromQuery] RideGetBinding binding) => Ok(await _rideHandler.GetRides(binding));
```

Handlers extend `Handler<THandler>`, take `IHandlerContext<THandler>`, and open a context with `GetMainContext()` inside `using`. Register each new handler in `Startup.ConfigureServices` with `services.AddHandler<IRideHandler, RideHandler>()`.

The current user is `UserId` on the handler, resolved from the Keycloak email claim. For types that extend `UserEntity`, set `entity.UserId = UserId` on create and filter reads with `WhereUser(UserId)`.

## Models

- Request bodies and query objects go in `Model/Binding/{Area}`. Date-range filters extend `FilteredBinding`.
- Responses go in `Model/View/{Area}` and are constructed from the entity (`new View.Ride.Ride(entity)`).
- Entities go in `Model/Database/Main/{Area}`, with `[Table]` and schema. Add a `DbSet` and any relationship in `MainContext`.
- Map a binding to an entity in `Business/MapExtensions` (`ToEntity`). Resolve public string ids with `DbSet.GetId`. Do not expose integer `Id` values on bindings or views when a `ValueId` exists.
- Query filters belong in `Data/Extensions/Entities` (`IQueryable<Ride>.Where(RideGetBinding)`), using `WhereIf` for optional predicates.

There are no EF migrations. `MainContext` maps an existing SQL Server database (`CONNECTION_STRING_MAIN`). Do not generate migrations.

Raw SQL lives in `src/ProjectIvy.Data/Sql/Main/Scripts` and must be listed as an `EmbeddedResource` in `ProjectIvy.Data.csproj`. Load it with `SqlLoader`.

## MCP

Tools live in `src/ProjectIvy.Api/Mcp`, use `[McpServerToolType]` / `[McpServerTool]`, and call an existing handler. `WithToolsFromAssembly()` picks them up. Do not query `MainContext` from a tool.

## Commands

```bash
dotnet build ProjectIvy.sln
dotnet test test/ProjectIvy.Data.Test/ProjectIvy.Data.Test.csproj
```

Configuration comes from environment variables (`CONNECTION_STRING_MAIN`, `OAUTH_AUTHORITY`, and the others read in `Startup`). Do not commit secrets or connection strings.
