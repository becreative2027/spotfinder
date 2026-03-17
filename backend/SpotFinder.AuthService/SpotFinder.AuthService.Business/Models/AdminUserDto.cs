namespace SpotFinder.AuthService.Business.Models;

public record AdminUserDto(
    Guid Id,
    string Email,
    string? FullName,
    string Role,
    bool IsActive,
    DateTime CreatedAt
);

public record PagedResult<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);
