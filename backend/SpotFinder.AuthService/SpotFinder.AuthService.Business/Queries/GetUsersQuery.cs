using MediatR;
using SpotFinder.AuthService.Business.Models;
using SpotFinder.AuthService.Data.Repositories;

namespace SpotFinder.AuthService.Business.Queries;

public record GetUsersQuery(int Page = 1, int PageSize = 20) : IRequest<PagedResult<AdminUserDto>>;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<AdminUserDto>>
{
    private readonly IUserRepository _repo;

    public GetUsersQueryHandler(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<PagedResult<AdminUserDto>> Handle(GetUsersQuery request, CancellationToken ct)
    {
        var (items, total) = await _repo.GetPagedAsync(request.Page, request.PageSize, ct);
        var dtos = items.Select(u => new AdminUserDto(u.Id, u.Email, u.FullName, u.Role, u.IsActive, u.CreatedAt));
        int totalPages = (int)Math.Ceiling(total / (double)request.PageSize);
        return new PagedResult<AdminUserDto>(dtos, total, request.Page, request.PageSize, totalPages);
    }
}
