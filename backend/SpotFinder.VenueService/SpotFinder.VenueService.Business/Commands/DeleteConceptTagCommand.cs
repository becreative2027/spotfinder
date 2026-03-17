using MediatR;
using SpotFinder.VenueService.Data.Repositories;

namespace SpotFinder.VenueService.Business.Commands;

public record DeleteConceptTagCommand(int Id) : IRequest<bool>;

public class DeleteConceptTagCommandHandler : IRequestHandler<DeleteConceptTagCommand, bool>
{
    private readonly IConceptTagRepository _repo;

    public DeleteConceptTagCommandHandler(IConceptTagRepository repo)
    {
        _repo = repo;
    }

    public async Task<bool> Handle(DeleteConceptTagCommand request, CancellationToken ct)
    {
        var deleted = await _repo.DeleteAsync(request.Id, ct);
        if (!deleted) return false;
        await _repo.SaveChangesAsync(ct);
        return true;
    }
}
