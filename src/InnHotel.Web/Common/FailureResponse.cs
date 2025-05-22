namespace InnHotel.Web.Common;

/// <summary>
/// Standard API error response model.
/// </summary>
public record FailureResponse
{
    /// <summary>
    /// The HTTP status code associated with the error.
    /// </summary>
    public int Status { get; init; }

    /// <summary>
    /// A human-readable error message describing what went wrong.
    /// </summary>
    public string Message { get; init; }

    /// <summary>
    /// An optional list of detailed error messages (e.g. for validation errors).
    /// </summary>
    public List<string>? Details { get; init; }

    public FailureResponse(int status, string message, List<string>? details = null)
    {
        Status = status;
        Message = message;
        Details = details;
    }
}
