#!/usr/bin/env dotnet-script
/*
|--------------------------------------------------------------------------
| InnHotel API Operation Generator
|--------------------------------------------------------------------------
|
| Quick Start:
|   dotnet script ./scripts/create-operation.csx -- --type <operation> --entity <singular> --plural <plural>
|
| Example:
|   dotnet script ./scripts/create-operation.csx -- --type Delete --entity Branch --plural Branches
|
| ⚠️ Important:
|   For Delete operations, manual DI registration is required in:
|   src/InnHotel.Infrastructure/InfrastructureServiceExtensions.cs
|
| For detailed documentation, see:
|   docs/dev/create-operation.md
|
*/

#r "nuget: System.CommandLine, 2.0.0-beta4"  // Pinned version for better caching

using System.CommandLine;
using System.CommandLine.Invocation;
using static System.Console;
using System.Text.RegularExpressions;

// Operation type enum
public enum OperationType
{
    Create,
    List,
    GetById,
    Update,
    Delete
}

// Configuration
public static class Config
{
    // Get the project root directory (current working directory)
    public static string ProjectRoot = Directory.GetCurrentDirectory();
    public static string SrcRoot = Path.Combine(ProjectRoot, "src");
    public static string WebRoot = Path.Combine(SrcRoot, "InnHotel.Web");
    public static string UseCasesRoot = Path.Combine(SrcRoot, "InnHotel.UseCases");
    public static string CoreRoot = Path.Combine(SrcRoot, "InnHotel.Core");
    public static string InfrastructureRoot = Path.Combine(SrcRoot, "InnHotel.Infrastructure");

    static Config()
    {
        // Validate project structure
        if (!Directory.Exists(SrcRoot) || !Directory.Exists(WebRoot) || !Directory.Exists(UseCasesRoot))
        {
            throw new DirectoryNotFoundException(
                $"Project structure not found at {ProjectRoot}. Ensure you're running the script from the project root directory."
            );
        }
    }
}

// Template generator class
public class TemplateGenerator
{
    private readonly string _entityNameSingular;
    private readonly string _entityNamePlural;
    private readonly OperationType _operationType;

    public TemplateGenerator(string entityNameSingular, string entityNamePlural, OperationType operationType)
    {
        _entityNameSingular = entityNameSingular;
        _entityNamePlural = entityNamePlural;
        _operationType = operationType;
    }

    private static string GetPluralName(string name)
    {
        // If name already ends in 's', return as is
        if (name.EndsWith("s", StringComparison.OrdinalIgnoreCase))
            return name;

        // Basic English pluralization rules
        if (name.EndsWith("y", StringComparison.OrdinalIgnoreCase))
            return name.Substring(0, name.Length - 1) + "ies";
        
        return name + "s";
    }

    private string GetSingularName(string name)
    {
        // If name ends in 'ies', convert to y
        if (name.EndsWith("ies", StringComparison.OrdinalIgnoreCase))
            return name.Substring(0, name.Length - 3) + "y";
        
        // If name ends in 's', remove it
        if (name.EndsWith("s", StringComparison.OrdinalIgnoreCase))
            return name.Substring(0, name.Length - 1);
        
        return name;
    }

    public string GetEndpointContent()
    {
        return _operationType switch
        {
            OperationType.Create => $@"
using Ardalis.Result.AspNetCore;
using InnHotel.UseCases.{_entityNamePlural}.Create;

namespace InnHotel.Web.{_entityNamePlural};

/// <summary>
/// Create a new {_entityNameSingular}
/// </summary>
public class Create(IMediator _mediator)
  : Endpoint<Create{_entityNameSingular}Request, Create{_entityNameSingular}Response>
{{
  public override void Configure()
  {{
    Post(Create{_entityNameSingular}Request.Route);
    Summary(s =>
    {{
      s.ExampleRequest = new Create{_entityNameSingular}Request
      {{
        // Add example properties
      }};
    }});
  }}

  public override async Task HandleAsync(
    Create{_entityNameSingular}Request request,
    CancellationToken cancellationToken)
  {{
    var result = await _mediator.Send(
      new Create{_entityNameSingular}Command(
        // Map properties from request
      ),
      cancellationToken
    );

    if (result.IsSuccess)
    {{
      Response = new Create{_entityNameSingular}Response(
        result.Value,
        // Map properties from request
      );
      return;
    }}

    await SendResultAsync(result.ToMinimalApiResult());
  }}
}}",
            
            OperationType.List => $@"
using InnHotel.UseCases.{_entityNamePlural}.List;
using InnHotel.Web.Common;

namespace InnHotel.Web.{_entityNamePlural};

/// <summary>
/// List all {_entityNameSingular}s with pagination support.
/// </summary>
/// <remarks>
/// Returns a paginated list of {_entityNameSingular} records.
/// </remarks>
public class List(IMediator _mediator)
    : Endpoint<PaginationRequest, object>
{{
    public override void Configure()
    {{
        Get(List{_entityNameSingular}sRequest.Route);
        Summary(s =>
        {{
            s.Summary = ""Get paginated list of {_entityNameSingular.ToLower()}s"";
            s.Description = ""Returns a paginated list of {_entityNameSingular.ToLower()} records with optional page number and size parameters"";
        }});
    }}

    public override async Task HandleAsync(PaginationRequest request, CancellationToken cancellationToken)
    {{
        var query = new List{_entityNameSingular}sQuery(request.PageNumber, request.PageSize);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsSuccess)
        {{
            var (items, totalCount) = result.Value;
            var {_entityNameSingular.ToLower()}Records = items.Select(e => 
                new {_entityNameSingular}Record(
                    // Map properties from DTO to Record
                ))
                .ToList();

            var response = new PagedResponse<{_entityNameSingular}Record>(
                {_entityNameSingular.ToLower()}Records, 
                totalCount, 
                request.PageNumber, 
                request.PageSize);

            await SendOkAsync(response, cancellationToken);
            return;
        }}

        await SendAsync(new FailureResponse(500, ""An unexpected error occurred.""), 
            statusCode: 500, 
            cancellation: cancellationToken);
    }}
}}",
            
            OperationType.GetById => $@"
using InnHotel.UseCases.{_entityNamePlural}.Get;
using InnHotel.Web.Common;

namespace InnHotel.Web.{_entityNamePlural};

/// <summary>
/// Get a {_entityNameSingular} by integer ID.
/// </summary>
/// <remarks>
/// Takes a positive integer ID and returns a matching {_entityNameSingular} record.
/// </remarks>
public class GetById(IMediator _mediator)
  : Endpoint<Get{_entityNameSingular}ByIdRequest, object>
{{
  public override void Configure()
  {{
    Get(Get{_entityNameSingular}ByIdRequest.Route);
  }}

  public override async Task HandleAsync(Get{_entityNameSingular}ByIdRequest request,
    CancellationToken cancellationToken)
  {{
    var query = new Get{_entityNameSingular}Query(request.{_entityNameSingular}Id);

    var result = await _mediator.Send(query, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {{
      var error = new FailureResponse(404, $""{_entityNameSingular} with ID {{request.{_entityNameSingular}Id}} not found"");
      await SendAsync(error, statusCode: 404, cancellation: cancellationToken);
      return;
    }}

    if (result.IsSuccess)
    {{
      Response = new {_entityNameSingular}Record(
        result.Value.Id
        // Map other properties from DTO
      );
      await SendOkAsync(Response, cancellationToken);
      return;
    }}

    await SendAsync(new FailureResponse(500, ""An unexpected error occurred.""), statusCode: 500, cancellation: cancellationToken);
  }}
}}",
            
            OperationType.Update => $@"
using InnHotel.UseCases.{_entityNamePlural}.Update;
using InnHotel.Web.Common;

namespace InnHotel.Web.{_entityNamePlural};

/// <summary>
/// Update an existing {_entityNameSingular}.
/// </summary>
/// <remarks>
/// Update an existing {_entityNameSingular} by providing updated details.
/// </remarks>
public class Update(IMediator _mediator)
    : Endpoint<Update{_entityNameSingular}Request, object>
{{
    public override void Configure()
    {{
        Put(Update{_entityNameSingular}Request.Route);
    }}

    public override async Task HandleAsync(
        Update{_entityNameSingular}Request request,
        CancellationToken cancellationToken)
    {{
        var command = new Update{_entityNameSingular}Command(
            request.{_entityNameSingular}Id
            // Map other properties from request
        );

        var result = await _mediator.Send(command, cancellationToken);

        if (result.Status == ResultStatus.NotFound)
        {{
            await SendAsync(
                new FailureResponse(404, $""{_entityNameSingular} with ID {{request.{_entityNameSingular}Id}} not found""),
                statusCode: 404,
                cancellation: cancellationToken);
            return;
        }}

        if (result.Status == ResultStatus.Error)
        {{
            await SendAsync(
                new FailureResponse(400, result.Errors.First()),
                statusCode: 400,
                cancellation: cancellationToken);
            return;
        }}

        if (result.IsSuccess)
        {{
            var record = new {_entityNameSingular}Record(
                result.Value.Id
                // Map other properties from result
            );

            await SendAsync(new {{ status = 200, message = ""{_entityNameSingular} updated successfully"", data = record }},
                statusCode: 200,
                cancellation: cancellationToken);
            return;
        }}

        await SendAsync(
            new FailureResponse(500, ""An unexpected error occurred.""),
            statusCode: 500,
            cancellation: cancellationToken);
    }}
}}",
            
            OperationType.Delete => $@"using InnHotel.UseCases.{_entityNamePlural}.Delete;
using InnHotel.Web.Common;

namespace InnHotel.Web.{_entityNamePlural};

/// <summary>
/// Delete a {_entityNameSingular}.
/// </summary>
/// <remarks>
/// Delete a {_entityNameSingular} by providing a valid integer id.
/// </remarks>
public class Delete(IMediator _mediator)
  : Endpoint<Delete{_entityNameSingular}Request>
{{
  public override void Configure()
  {{
    Delete(Delete{_entityNameSingular}Request.Route);
  }}

  public override async Task HandleAsync(
    Delete{_entityNameSingular}Request request,
    CancellationToken cancellationToken)
  {{
    var command = new Delete{_entityNameSingular}Command(request.{_entityNameSingular}Id);

    var result = await _mediator.Send(command, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {{
      var error = new FailureResponse(404, $""{_entityNameSingular} with ID {{request.{_entityNameSingular}Id}} not found"");
      await SendAsync(error, statusCode: 404, cancellation: cancellationToken);
      return;
    }}

    if (result.IsSuccess)
    {{
      await SendAsync(new {{ status = 200, message = $""{_entityNameSingular} with ID {{request.{_entityNameSingular}Id}} was successfully deleted"" }}, 
        statusCode: 200, 
        cancellation: cancellationToken);
      return;
    }}

    await SendAsync(new FailureResponse(500, ""An unexpected error occurred.""), statusCode: 500, cancellation: cancellationToken);
  }}
}}",
            
            _ => throw new ArgumentException($"Unknown operation type: {_operationType}")
        };
    }

    public string GetCommandContent()
    {
        return _operationType switch
        {
            OperationType.Create => $@"
namespace InnHotel.UseCases.{_entityNamePlural}.Create;

public record Create{_entityNameSingular}Command() : ICommand<Result<int>>;",
            
            OperationType.List => $@"
namespace InnHotel.UseCases.{_entityNamePlural}.List;

public record List{_entityNameSingular}sQuery(int PageNumber, int PageSize) 
    : IQuery<Result<(List<{_entityNameSingular}DTO> Items, int TotalCount)>>;",
            
            OperationType.GetById => $@"
namespace InnHotel.UseCases.{_entityNamePlural}.Get;

public record Get{_entityNameSingular}Query(int {_entityNameSingular}Id) : IQuery<Result<{_entityNameSingular}DTO>>;",
            
            OperationType.Update => $@"
namespace InnHotel.UseCases.{_entityNamePlural}.Update;

public record Update{_entityNameSingular}Command(
    int {_entityNameSingular}Id
    // Add other properties
) : ICommand<Result<{_entityNameSingular}DTO>>;",
            
            OperationType.Delete => $@"namespace InnHotel.UseCases.{_entityNamePlural}.Delete;

public record Delete{_entityNameSingular}Command(int {_entityNameSingular}Id) : ICommand<Result>;",
            
            _ => throw new ArgumentException($"Unknown operation type: {_operationType}")
        };
    }

    public string GetHandlerContent()
    {
        return _operationType switch
        {
            OperationType.Create => $@"
using InnHotel.Core.{_entityNameSingular}Aggregate;

namespace InnHotel.UseCases.{_entityNamePlural}.Create;

internal class Create{_entityNameSingular}Handler(IRepository<{_entityNameSingular}> _repository)
  : ICommandHandler<Create{_entityNameSingular}Command, Result<int>>
{{   
    public async Task<Result<int>> Handle(Create{_entityNameSingular}Command request, CancellationToken cancellationToken)
    {{
        // Add validation if needed (e.g., check for duplicates)
        
        var entity = new {_entityNameSingular}(
            // Map properties from request
        );

        var created = await _repository.AddAsync(entity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success(created.Id);
    }}
}}",
            
            OperationType.List => $@"
using InnHotel.Core.{_entityNameSingular}Aggregate;

namespace InnHotel.UseCases.{_entityNamePlural}.List;

public class List{_entityNameSingular}sHandler(IReadRepository<{_entityNameSingular}> _repository)
    : IQueryHandler<List{_entityNameSingular}sQuery, Result<(List<{_entityNameSingular}DTO> Items, int TotalCount)>>
{{
    public async Task<Result<(List<{_entityNameSingular}DTO> Items, int TotalCount)>> Handle(
        List{_entityNameSingular}sQuery request, 
        CancellationToken cancellationToken)
    {{
        var totalCount = await _repository.CountAsync(cancellationToken);
        
        var entities = await _repository.ListAsync(cancellationToken);
        var pagedEntities = entities
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        var dtos = pagedEntities.Select(entity => new {_entityNameSingular}DTO(
            // Map properties from entity to DTO
        )).ToList();

        return (dtos, totalCount);
    }}
}}",
            
            OperationType.GetById => $@"
using InnHotel.Core.{_entityNameSingular}Aggregate;
using InnHotel.Core.{_entityNameSingular}Aggregate.Specifications;

namespace InnHotel.UseCases.{_entityNamePlural}.Get;

public class Get{_entityNameSingular}Handler(IReadRepository<{_entityNameSingular}> _repository)
  : IQueryHandler<Get{_entityNameSingular}Query, Result<{_entityNameSingular}DTO>>
{{
  public async Task<Result<{_entityNameSingular}DTO>> Handle(Get{_entityNameSingular}Query request, CancellationToken cancellationToken)
  {{
    var spec = new {_entityNameSingular}ByIdSpec(request.{_entityNameSingular}Id);
    var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (entity == null) return Result.NotFound();

    return new {_entityNameSingular}DTO(
      entity.Id
      // Map other properties from entity
    );
  }}
}}",
            
            OperationType.Update => $@"
using InnHotel.Core.{_entityNameSingular}Aggregate;
using Microsoft.EntityFrameworkCore;

namespace InnHotel.UseCases.{_entityNamePlural}.Update;

public class Update{_entityNameSingular}Handler(IRepository<{_entityNameSingular}> _repository)
    : ICommandHandler<Update{_entityNameSingular}Command, Result<{_entityNameSingular}DTO>>
{{
    public async Task<Result<{_entityNameSingular}DTO>> Handle(Update{_entityNameSingular}Command request, CancellationToken cancellationToken)
    {{
        var spec = new {_entityNameSingular}ByIdSpec(request.{_entityNameSingular}Id);
        var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
        
        if (entity == null)
            return Result.NotFound();

        // Add any unique constraint checks here if needed
        
        entity.UpdateDetails(
            // Map properties from request
        );

        try
        {{
            await _repository.UpdateAsync(entity, cancellationToken);
            
            return new {_entityNameSingular}DTO(
                entity.Id
                // Map other properties from entity
            );
        }}
        catch (DbUpdateException ex)
        {{
            // Handle unique constraint violations
            return Result.Error(""A validation error occurred."");
        }}
    }}
}}",
            
            OperationType.Delete => $@"namespace InnHotel.UseCases.{_entityNamePlural}.Delete;
using InnHotel.Core.Interfaces;

public class Delete{_entityNameSingular}Handler(IDelete{_entityNameSingular}Service _delete{_entityNameSingular}Service)
  : ICommandHandler<Delete{_entityNameSingular}Command, Result>
{{
  public async Task<Result> Handle(Delete{_entityNameSingular}Command request, CancellationToken cancellationToken) =>
    await _delete{_entityNameSingular}Service.Delete{_entityNameSingular}(request.{_entityNameSingular}Id);
}}",
            
            _ => throw new ArgumentException($"Unknown operation type: {_operationType}")
        };
    }

    public string GetRequestContent()
    {
        var paramName = $"{_entityNameSingular}Id";
        return _operationType switch
        {
            OperationType.Delete => $@"namespace InnHotel.Web.{_entityNamePlural};

public record Delete{_entityNameSingular}Request
{{
  public const string Route = ""api/{_entityNamePlural}/{{{_entityNameSingular}Id:int}}"";
  public static string BuildRoute(int {_entityNameSingular.ToLower()}Id) => Route.Replace(""{{{_entityNameSingular}Id:int}}"", {_entityNameSingular.ToLower()}Id.ToString());

  public int {_entityNameSingular}Id {{ get; set; }}
}}

",
            OperationType.List => $@"
namespace InnHotel.Web.{_entityNamePlural};

public class List{_entityNameSingular}sRequest
{{
    public const string Route = ""api/{_entityNamePlural}"";
    public static string BuildRoute() => Route;
}}",
            OperationType.Create => $@"
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace InnHotel.Web.{_entityNamePlural};

/// <summary>
/// Request DTO to create a new {_entityNameSingular}
/// </summary>
public class Create{_entityNameSingular}Request
{{
    public const string Route = ""api/{_entityNameSingular.ToLower()}"";

    // Add properties with validation attributes
}}

/// <summary>
/// Response DTO returned after creating a {_entityNameSingular}
/// </summary>
public class Create{_entityNameSingular}Response
{{
    public Create{_entityNameSingular}Response(int id)
    {{
        Id = id;
        // Initialize other properties
    }}

    public int Id {{ get; init; }}
    // Add other properties
}}",
            OperationType.Update => $@"
namespace InnHotel.Web.{_entityNamePlural};

public class Update{_entityNameSingular}Request
{{
    public const string Route = ""api/{_entityNamePlural}/{{{_entityNameSingular}Id:int}}"";
    public static string BuildRoute(int {_entityNameSingular.ToLower()}Id) => Route.Replace($""{{{_entityNameSingular}Id:int}}"", {_entityNameSingular.ToLower()}Id.ToString());

    public int {_entityNameSingular}Id {{ get; set; }}
    // Add other properties with proper defaults
}}",
            OperationType.GetById => $@"
namespace InnHotel.Web.{_entityNamePlural};

public class Get{_entityNameSingular}ByIdRequest
{{
  public const string Route = ""api/{_entityNamePlural}/{{{_entityNameSingular}Id:int}}"";
  public static string BuildRoute(int {_entityNameSingular.ToLower()}Id) => Route.Replace($""{{{_entityNameSingular}Id:int}}"", {_entityNameSingular.ToLower()}Id.ToString());

  public int {_entityNameSingular}Id {{ get; set; }}
}}",
            _ => throw new ArgumentException($"Unknown operation type: {_operationType}")
        };
    }

    public string GetValidatorContent()
    {
        return _operationType switch
        {
            OperationType.Delete => $@"using FastEndpoints;
using FluentValidation;
using global::InnHotel.Web.{_entityNamePlural};

namespace InnHotel.Web.{_entityNamePlural};

/// <summary>
/// See: https://fast-endpoints.com/docs/validation
/// </summary>
public class Delete{_entityNameSingular}Validator : Validator<Delete{_entityNameSingular}Request>
{{
  public Delete{_entityNameSingular}Validator()
  {{
    RuleFor(x => x.{_entityNameSingular}Id)
      .GreaterThan(0);
  }}
}}
",
            
            // Add other validator templates for Create, Update, etc.
            _ => string.Empty
        };
    }

    public string GetDeleteServiceInterfaceContent()
    {
        return $@"namespace InnHotel.Core.Interfaces;

public interface IDelete{_entityNameSingular}Service
{{
  // This service and method exist to provide a place in which to fire domain events
  // when deleting this aggregate root entity
  public Task<Result> Delete{_entityNameSingular}(int {_entityNameSingular.ToLower()}Id);
}}
";
    }

    public string GetDeletedEventContent()
    {
        return $@"namespace InnHotel.Core.{_entityNameSingular}Aggregate.Events;

/// <summary>
/// A domain event that is dispatched whenever a {_entityNameSingular.ToLower()} is deleted.
/// The Delete{_entityNameSingular}Service is used to dispatch this event.
/// </summary>
internal sealed class {_entityNameSingular}DeletedEvent(int {_entityNameSingular.ToLower()}Id) : DomainEventBase
{{
  public int {_entityNameSingular.ToLower()}Id {{ get; init; }} = {_entityNameSingular.ToLower()}Id;
}}
";
    }

    public void CreateFiles()
    {
        // Create Web layer files
        var webEntityDir = Path.Combine(Config.WebRoot, _entityNamePlural);
        FileOperations.EnsureDirectory(webEntityDir);

        // Create Use Cases layer files
        var useCasesEntityDir = Path.Combine(Config.UseCasesRoot, _entityNamePlural, _operationType.ToString());
        FileOperations.EnsureDirectory(useCasesEntityDir);

        // Create Core layer files for Delete operation
        if (_operationType == OperationType.Delete)
        {
            // Create the service
            var servicesDir = Path.Combine(Config.CoreRoot, "Services");
            FileOperations.EnsureDirectory(servicesDir);
            FileOperations.CreateFile(
                Path.Combine(servicesDir, $"Delete{_entityNameSingular}Service.cs"),
                GetDeleteServiceContent()
            );

            // Create the domain event
            var eventsDir = Path.Combine(Config.CoreRoot, $"{_entityNameSingular}Aggregate", "Events");
            FileOperations.EnsureDirectory(eventsDir);
            FileOperations.CreateFile(
                Path.Combine(eventsDir, $"{_entityNameSingular}DeletedEvent.cs"),
                GetDeletedEventContent()
            );
        }

        // Generate and write files
        FileOperations.CreateFile(
            Path.Combine(webEntityDir, $"{_operationType}.cs"),
            GetEndpointContent()
        );

        var commandFileName = _operationType switch
        {
            OperationType.List => $"List{_entityNameSingular}Query.cs",
            OperationType.GetById => $"Get{_entityNameSingular}Query.cs",
            _ => $"{_operationType}{_entityNameSingular}Command.cs"
        };

        FileOperations.CreateFile(
            Path.Combine(useCasesEntityDir, commandFileName),
            GetCommandContent()
        );

        FileOperations.CreateFile(
            Path.Combine(useCasesEntityDir, $"{_operationType}{_entityNameSingular}Handler.cs"),
            GetHandlerContent()
        );

        // Create request/response files in Web layer
        var requestContent = GetRequestContent();
        if (!string.IsNullOrEmpty(requestContent))
        {
            var requestFileName = _operationType switch
            {
                OperationType.Delete => $"{_operationType}.Delete{_entityNameSingular}Request.cs",
                OperationType.GetById => $"{_operationType}.Get{_entityNameSingular}ByIdRequest.cs",
                OperationType.Update => $"{_operationType}.Update{_entityNameSingular}Request.cs",
                OperationType.Create => $"{_operationType}.Create{_entityNameSingular}Request.cs",
                OperationType.List => $"{_operationType}.List{_entityNameSingular}Request.cs",
                _ => throw new ArgumentException($"Unknown operation type: {_operationType}")
            };

            FileOperations.CreateFile(
                Path.Combine(webEntityDir, requestFileName),
                requestContent
            );
        }

        // Create validator files in Web layer
        var validatorContent = GetValidatorContent();
        if (!string.IsNullOrEmpty(validatorContent))
        {
            var validatorFileName = _operationType switch
            {
                OperationType.Delete => $"{_operationType}.Delete{_entityNameSingular}Validator.cs",
                OperationType.GetById => $"{_operationType}.Get{_entityNameSingular}ByIdValidator.cs",
                OperationType.Update => $"{_operationType}.Update{_entityNameSingular}Validator.cs",
                OperationType.Create => $"{_operationType}.Create{_entityNameSingular}Validator.cs",
                _ => throw new ArgumentException($"Unknown operation type: {_operationType}")
            };

            FileOperations.CreateFile(
                Path.Combine(webEntityDir, validatorFileName),
                validatorContent
            );
        }

        // Create delete service interface if this is a Delete operation
        if (_operationType == OperationType.Delete)
        {
            FileOperations.CreateFile(
                Path.Combine(Config.CoreRoot, "Interfaces", $"IDelete{_entityNameSingular}Service.cs"),
                GetDeleteServiceInterfaceContent()
            );
        }

        WriteLine($"\nOperation '{_operationType}' for {_entityNameSingular} has been created successfully!");
        WriteLine("Remember to:");
        WriteLine("1. Implement the handler logic");
        WriteLine("2. Add necessary properties to request/response models");
        WriteLine("3. Add validation if needed");
        WriteLine("4. Update repository interfaces if required");
        if (_operationType == OperationType.Delete)
        {
            WriteLine("5. Implement the delete service in the Infrastructure layer");
        }
    }

    public string GetDeleteServiceContent()
    {
        return $@"namespace InnHotel.Core.Services;
using InnHotel.Core.{_entityNameSingular}Aggregate.Events;
using InnHotel.Core.{_entityNameSingular}Aggregate;

using InnHotel.Core.Interfaces;


/// <summary>
/// This is here mainly so there's an example of a domain service
/// and also to demonstrate how to fire domain events from a service.
/// </summary>
/// <param name=""_repository""></param>
/// <param name=""_mediator""></param>
/// <param name=""_logger""></param>
public class Delete{_entityNameSingular}Service(IRepository<{_entityNameSingular}> _repository,
  IMediator _mediator,
  ILogger<Delete{_entityNameSingular}Service> _logger) : IDelete{_entityNameSingular}Service
{{
  public async Task<Result> Delete{_entityNameSingular}(int {_entityNameSingular.ToLower()}Id)
  {{
    _logger.LogInformation(""Deleting Contributor {{contributorId}}"", {_entityNameSingular.ToLower()}Id);
    {_entityNameSingular}? aggregateToDelete = await _repository.GetByIdAsync({_entityNameSingular.ToLower()}Id);
    if (aggregateToDelete == null) return Result.NotFound();

    await _repository.DeleteAsync(aggregateToDelete);
    var domainEvent = new {_entityNameSingular}DeletedEvent({_entityNameSingular.ToLower()}Id);
    await _mediator.Publish(domainEvent);

    return Result.Success();
  }}
}}
";
    }
}

// File system operations
public static class FileOperations
{
    public static void EnsureDirectory(string path)
    {
        var fullPath = Path.GetFullPath(path);
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
            WriteLine($"Created directory: {fullPath}");
        }
    }

    public static void CreateFile(string path, string content)
    {
        var fullPath = Path.GetFullPath(path);
        if (!File.Exists(fullPath))
        {
            File.WriteAllText(fullPath, content.TrimStart());
            WriteLine($"Created file: {fullPath}");
        }
        else
        {
            WriteLine($"File already exists: {fullPath}");
        }
    }
}

// Command line parsing and execution
var operationTypeOption = new Option<OperationType>(
    "--type",
    "The type of operation to create (Create/List/GetById/Update/Delete)"
);

var entitySingularOption = new Option<string>(
    "--entity",
    "The singular name of the entity (e.g., Room, Guest, Branch)"
);

var entityPluralOption = new Option<string>(
    "--plural",
    "The plural name of the entity (e.g., Rooms, Guests, Branches)"
);

var rootCommand = new RootCommand("Creates operation files for InnHotel API")
{
    operationTypeOption,
    entitySingularOption,
    entityPluralOption
};

rootCommand.SetHandler((OperationType operationType, string entitySingular, string entityPlural) =>
{
    try
    {
        WriteLine($"Project Root: {Config.ProjectRoot}");
        WriteLine($"Creating {operationType} operation for {entitySingular} (plural: {entityPlural})...\n");

        // Create template generator with both singular and plural forms
        var generator = new TemplateGenerator(entitySingular, entityPlural, operationType);

        // Setup directories using plural form
        var webEntityDir = Path.Combine(Config.WebRoot, entityPlural);
        var useCasesEntityDir = Path.Combine(Config.UseCasesRoot, entityPlural, operationType.ToString());
        var interfacesDir = Path.Combine(Config.CoreRoot, "Interfaces");

        FileOperations.EnsureDirectory(webEntityDir);
        FileOperations.EnsureDirectory(useCasesEntityDir);
        FileOperations.EnsureDirectory(interfacesDir);

        // Generate and write files
        generator.CreateFiles();

        WriteLine($"\nOperation '{operationType}' for {entitySingular} has been created successfully!");
        WriteLine("Remember to:");
        WriteLine("1. Implement the handler logic");
        WriteLine("2. Add necessary properties to request/response models");
        WriteLine("3. Add validation if needed");
        WriteLine("4. Update repository interfaces if required");
        if (operationType == OperationType.Delete)
        {
            WriteLine("5. Implement the delete service in the Infrastructure layer");
        }
    }
    catch (Exception ex)
    {
        WriteLine($"Error: {ex.Message}");
        Environment.Exit(1);
    }
}, operationTypeOption, entitySingularOption, entityPluralOption);

return await rootCommand.InvokeAsync(Args.ToArray());