using MediatR;
using SpotFinder.AuthService.Business.Models;
using SpotFinder.AuthService.Data.Entities;
using SpotFinder.AuthService.Data.Repositories;

namespace SpotFinder.AuthService.Business.Commands;

public record AddVisitCommand(Guid UserId, Guid VenueId) : IRequest<VisitDto>;

public class AddVisitCommandHandler : IRequestHandler<AddVisitCommand, VisitDto>
{
    private readonly IUserInteractionRepository _repo;

    public AddVisitCommandHandler(IUserInteractionRepository repo)
    {
        _repo = repo;
    }

    public async Task<VisitDto> Handle(AddVisitCommand request, CancellationToken ct)
    {
        var visit = new UserVisit
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            VenueId = request.VenueId,
            VisitedAt = DateTime.UtcNow
        };

        await _repo.AddVisitAsync(visit, ct);
        await _repo.SaveChangesAsync(ct);

        return new VisitDto(visit.Id, visit.VenueId, visit.VisitedAt);
    }
}
