<h1>Operation Generator Scripts Documentation</h1>

## Overview
The operation generator scripts are tools designed to generate consistent CRUD operation files for the InnHotel API following clean architecture patterns and FastEndpoints conventions.

- [Overview](#overview)
- [Available Scripts](#available-scripts)
- [Prerequisites](#prerequisites)
- [Common Usage Pattern](#common-usage-pattern)
  - [Parameters](#parameters)
  - [Examples](#examples)
- [Create Operation Script](#create-operation-script)
  - [Generated Files](#generated-files)
  - [Features](#features)
- [List Operation Script](#list-operation-script)
  - [Generated Files](#generated-files-1)
  - [Features](#features-1)
- [Get By ID Operation Script](#get-by-id-operation-script)
  - [Generated Files](#generated-files-2)
  - [Features](#features-2)
- [Update Operation Script](#update-operation-script)
  - [Generated Files](#generated-files-3)
  - [Features](#features-3)
- [Delete Operation Script](#delete-operation-script)
  - [Generated Files](#generated-files-4)
  - [Features](#features-4)
  - [⚠️ Important: Service Registration](#️-important-service-registration)
- [Best Practices](#best-practices)
- [Troubleshooting](#troubleshooting)

## Available Scripts
1. [Create Operation](#create-operation-script) (`gen-create-operation.csx`)
2. [List Operation](#list-operation-script) (`gen-list-operation.csx`)
3. [Get By ID Operation](#get-by-id-operation-script) (`gen-get-by-id-operation.csx`)
4. [Update Operation](#update-operation-script) (`gen-update-operation.csx`)
5. [Delete Operation](#delete-operation-script) (`gen-delete-operation.csx`)

## Prerequisites
- .NET 7.0 or later
- dotnet-script tool installed globally
  ```bash
  dotnet tool install -g dotnet-script
  ```

## Common Usage Pattern
All scripts follow the same command pattern:
```bash
dotnet script ./scripts/<script-name>.csx -- --entity <singular> --plural <plural>
```

### Parameters
- `--entity`: The singular form of the entity name (e.g., Room, Guest, Branch)
- `--plural`: The plural form of the entity name (e.g., Rooms, Guests, Branches)

### Examples
```bash
# Create operation
dotnet script ./scripts/gen-create-operation.csx -- --entity Room --plural Rooms

# List operation
dotnet script ./scripts/gen-list-operation.csx -- --entity Room --plural Rooms

# Get By ID operation
dotnet script ./scripts/gen-get-by-id-operation.csx -- --entity Room --plural Rooms

# Update operation
dotnet script ./scripts/gen-update-operation.csx -- --entity Room --plural Rooms

# Delete operation
dotnet script ./scripts/gen-delete-operation.csx -- --entity Room --plural Rooms
```

## Create Operation Script

### Generated Files
1. Web Layer (`src/InnHotel.Web/{Plural}/`)
   - `Create.cs` - FastEndpoints implementation
   - `Create.Create{Entity}Request.cs` - Request model
   - `Create.Create{Entity}Validator.cs` - Request validation

2. Use Cases Layer (`src/InnHotel.UseCases/{Plural}/Create/`)
   - `Create{Entity}Command.cs` - Command definition
   - `Create{Entity}Handler.cs` - Command handler

### Features
- Input validation with FluentValidation
- Entity relationship handling
- Duplicate checking
- Error handling
- API documentation
- Authorization requirements

## List Operation Script

### Generated Files
1. Web Layer (`src/InnHotel.Web/{Plural}/`)
   - `List.cs` - FastEndpoints implementation
   - `List.List{Entity}Request.cs` - Request model

2. Use Cases Layer (`src/InnHotel.UseCases/{Plural}/List/`)
   - `List{Entity}Query.cs` - Query definition
   - `List{Entity}Handler.cs` - Query handler

3. Core Layer (`src/InnHotel.Core/{Entity}Aggregate/Specifications/`)
   - `{Entity}WithDetailsSpec.cs` - Specification for loading relationships

### Features
- Pagination support
- Relationship loading
- Response mapping
- API documentation
- Performance optimization

## Get By ID Operation Script

### Generated Files
1. Web Layer (`src/InnHotel.Web/{Plural}/`)
   - `GetById.cs` - FastEndpoints implementation
   - `GetById.Get{Entity}ByIdRequest.cs` - Request model
   - `GetById.Get{Entity}ByIdValidator.cs` - Request validation

2. Use Cases Layer (`src/InnHotel.UseCases/{Plural}/Get/`)
   - `Get{Entity}Query.cs` - Query definition
   - `Get{Entity}Handler.cs` - Query handler

3. Core Layer (`src/InnHotel.Core/{Entity}Aggregate/Specifications/`)
   - `{Entity}ByIdSpec.cs` - Specification for loading by ID

### Features
- Input validation
- Relationship loading
- Not found handling
- API documentation
- Authorization requirements

## Update Operation Script

### Generated Files
1. Web Layer (`src/InnHotel.Web/{Plural}/`)
   - `Update.cs` - FastEndpoints implementation
   - `Update.Update{Entity}Request.cs` - Request model
   - `Update.Update{Entity}Validator.cs` - Request validation

2. Use Cases Layer (`src/InnHotel.UseCases/{Plural}/Update/`)
   - `Update{Entity}Command.cs` - Command definition
   - `Update{Entity}Handler.cs` - Command handler

### Features
- Input validation
- Relationship validation
- Concurrency handling
- Error handling
- API documentation
- Authorization requirements

## Delete Operation Script

### Generated Files
1. Web Layer (`src/InnHotel.Web/{Plural}/`)
   - `Delete.cs` - FastEndpoints implementation
   - `Delete.Delete{Entity}Request.cs` - Request model
   - `Delete.Delete{Entity}Validator.cs` - Request validation

2. Use Cases Layer (`src/InnHotel.UseCases/{Plural}/Delete/`)
   - `Delete{Entity}Command.cs` - Command definition
   - `Delete{Entity}Handler.cs` - Command handler

3. Core Layer
   - `src/InnHotel.Core/Interfaces/IDelete{Entity}Service.cs`
   - `src/InnHotel.Core/Services/Delete{Entity}Service.cs`
   - `src/InnHotel.Core/{Entity}Aggregate/Events/{Entity}DeletedEvent.cs`

### Features
- Input validation
- Relationship checking
- Cascade delete handling
- Domain events
- Error handling
- API documentation
- Authorization requirements

### ⚠️ Important: Service Registration
After generating a Delete operation, you must register the delete service:

1. Open `src/InnHotel.Infrastructure/InfrastructureServiceExtensions.cs`
2. Add to `AddInfrastructureServices`:
   ```csharp
   services.AddScoped<IDelete{Entity}Service, Delete{Entity}Service>();
   ```

## Best Practices
1. Use PascalCase for entity names
2. Follow existing patterns in the codebase
3. Review generated files for customization needs
4. Add appropriate authorization attributes
5. Consider adding caching where appropriate
6. Add proper validation rules
7. Include comprehensive API documentation

## Troubleshooting
1. Ensure you're in the project root directory
2. Verify all required directories exist
3. Check file permissions
4. Validate entity names follow C# conventions
5. Check for conflicting files before generation
6. Ensure all required packages are installed
7. Verify proper service registration for Delete operations