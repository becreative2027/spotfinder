using MediatR;
using SpotFinder.VenueService.Data.Repositories;

namespace SpotFinder.VenueService.Business.Commands;

public record DeleteVenueCommand(Guid Id) : IRequest<bool>;

public class DeleteVenueCommandHandler : IRequestHandler<DeleteVenueCommand, bool>
{
    private readonly IVenueRepository _repo;
    public DeleteVenueCommandHandler(IVenueRepository repo) => _repo = repo;

    public async Task<bool> Handle(DeleteVenueCommand request, CancellationToken ct)
    {
        var venue = await _repo.GetByIdAsync(request.Id, ct);
        if (venue is null) return false;

        await _repo.DeleteAsync(venue, ct);
        await _repo.SaveChangesAsync(ct);
        return true;
    }
}
