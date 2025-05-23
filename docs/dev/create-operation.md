<h1>Operation Generator Script Documentation</h1>

## Overview
The operation generator script ([`create-operation.csx`](../../scripts/create-operation.csx)) is a tool designed to generate consistent CRUD operation files for the InnHotel API following clean architecture patterns and FastEndpoints conventions.

- [Overview](#overview)
- [Prerequisites](#prerequisites)
- [Usage](#usage)
  - [Basic Command](#basic-command)
  - [Parameters](#parameters)
  - [Examples](#examples)
- [Generated Files Structure](#generated-files-structure)
  - [1. Web Layer (`src/InnHotel.Web/{Plural}/`)](#1-web-layer-srcinnhotelwebplural)
  - [2. Use Cases Layer (`src/InnHotel.UseCases/{Plural}/{Operation}/`)](#2-use-cases-layer-srcinnhotelusecasespluraloperation)
  - [3. Core Layer (for Delete Operations)](#3-core-layer-for-delete-operations)
- [Naming Conventions](#naming-conventions)
  - [Singular Form Usage (--entity)](#singular-form-usage---entity)
  - [Plural Form Usage (--plural)](#plural-form-usage---plural)
- [⚠️ Special Considerations for Delete Operations](#️-special-considerations-for-delete-operations)
- [File Templates](#file-templates)
  - [Request Model Example](#request-model-example)
  - [Validator Example](#validator-example)
  - [Service Interface Example](#service-interface-example)
- [Best Practices](#best-practices)
- [Troubleshooting](#troubleshooting)

## Prerequisites
- .NET 7.0 or later
- dotnet-script tool installed globally
  ```bash
  dotnet tool install -g dotnet-script
  ```

## Usage

### Basic Command
```bash
dotnet script ./scripts/create-operation.csx -- --type <operation> --entity <singular> --plural <plural>
```

### Parameters
- `--type`: The type of operation to create
  - Valid values: `Create`, `List`, `GetById`, `Update`, `Delete`
- `--entity`: The singular form of the entity name (e.g., Room, Guest, Branch)
- `--plural`: The plural form of the entity name (e.g., Rooms, Guests, Branches)

### Examples
```bash
# Create a new room
dotnet script ./scripts/create-operation.csx -- --type Create --entity Room --plural Rooms

# List all guests
dotnet script ./scripts/create-operation.csx -- --type List --entity Guest --plural Guests

# Delete a branch
dotnet script ./scripts/create-operation.csx -- --type Delete --entity Branch --plural Branches

# Get a category by ID
dotnet script ./scripts/create-operation.csx -- --type GetById --entity Category --plural Categories
```

## Generated Files Structure

### 1. Web Layer (`src/InnHotel.Web/{Plural}/`)
- `{Operation}.cs`
  - FastEndpoints implementation
  - Handles HTTP request/response
  - Maps to use case commands/queries
- `{Operation}.{EntityName}Request.cs`
  - Request model with validation attributes
  - Route configuration
  - BuildRoute helper method
- `{Operation}.{EntityName}Validator.cs`
  - FluentValidation rules
  - Input validation logic

### 2. Use Cases Layer (`src/InnHotel.UseCases/{Plural}/{Operation}/`)
- Commands/Queries
  - `Create{Entity}Command.cs`
  - `List{Entity}Query.cs`
  - `Get{Entity}Query.cs`
  - `Update{Entity}Command.cs`
  - `Delete{Entity}Command.cs`
- Handlers
  - `Create{Entity}Handler.cs`
  - `List{Entity}Handler.cs`
  - `Get{Entity}Handler.cs`
  - `Update{Entity}Handler.cs`
  - `Delete{Entity}Handler.cs`

### 3. Core Layer (for Delete Operations)
- Interface (`src/InnHotel.Core/Interfaces/`)
  - `IDelete{Entity}Service.cs`
- Service Implementation (`src/InnHotel.Core/Services/`)
  - `Delete{Entity}Service.cs`
- Domain Event (`src/InnHotel.Core/{Entity}Aggregate/Events/`)
  - `{Entity}DeletedEvent.cs`

## Naming Conventions

### Singular Form Usage (--entity)
- Class names (e.g., CreateRoomCommand)
- Type names (e.g., RoomDTO)
- Single-entity operations
- File names
- Aggregate root names

### Plural Form Usage (--plural)
- Directory names (e.g., src/InnHotel.Web/Rooms/)
- API routes (e.g., /api/rooms)
- Collection endpoints
- List operation names

## ⚠️ Special Considerations for Delete Operations

After generating a Delete operation, you MUST perform this manual step:

1. Open `src/InnHotel.Infrastructure/InfrastructureServiceExtensions.cs`
2. Locate the `AddInfrastructureServices` method
3. Add the delete service registration:
   ```csharp
   services.AddScoped<IDelete{EntityName}Service, Delete{EntityName}Service>();
   ```
4. Add it alongside other similar registrations:
   ```csharp
   services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>))
          .AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>))
          .AddScoped<IDeleteGuestService, DeleteGuestService>()
          .AddScoped<IDelete{EntityName}Service, Delete{EntityName}Service>();  // <-- Add this line
   ```

This registration is required for:
- Proper dependency injection
- Domain event handling during deletion
- Clean architecture compliance

## File Templates

### Request Model Example
```csharp
namespace InnHotel.Web.{Plural};

public record Delete{Entity}Request
{
  public const string Route = "api/{Plural}/{{{Entity}Id:int}}";
  public static string BuildRoute(int {entity}Id) => Route.Replace("{{{Entity}Id:int}}", {entity}Id.ToString());

  public int {Entity}Id { get; set; }
}
```

### Validator Example
```csharp
namespace InnHotel.Web.{Plural};

public class Delete{Entity}Validator : Validator<Delete{Entity}Request>
{
  public Delete{Entity}Validator()
  {
    RuleFor(x => x.{Entity}Id)
      .GreaterThan(0);
  }
}
```

### Service Interface Example
```csharp
namespace InnHotel.Core.Interfaces;

public interface IDelete{Entity}Service
{
  public Task<Result> Delete{Entity}(int {entity}Id);
}
```

## Best Practices
1. Always use PascalCase for entity names
2. Keep consistent with existing patterns in the codebase
3. Follow the established directory structure
4. Don't skip the manual DI registration for Delete operations
5. Review generated files for any needed customizations

## Troubleshooting
1. Ensure you're in the project root directory when running the script
2. Verify all required directories exist
3. Check file permissions if creation fails
4. Validate entity names follow C# naming conventions
5. Ensure no conflicting files exist before generation