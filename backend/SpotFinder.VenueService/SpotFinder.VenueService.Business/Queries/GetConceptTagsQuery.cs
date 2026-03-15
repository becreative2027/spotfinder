using MediatR;
using SpotFinder.VenueService.Business.Models;
using SpotFinder.VenueService.Data.Repositories;

namespace SpotFinder.VenueService.Business.Queries;

public record GetConceptTagsQuery : IRequest<IEnumerable<ConceptTagDto>>;

public class GetConceptTagsQueryHandler : IRequestHandler<GetConceptTagsQuery, IEnumerable<ConceptTagDto>>
{
    private readonly IConceptTagRepository _repo;
    public GetConceptTagsQueryHandler(IConceptTagRepository repo) => _repo = repo;

    public async Task<IEnumerable<ConceptTagDto>> Handle(GetConceptTagsQuery request, CancellationToken ct)
    {
        var tags = await _repo.GetAllActiveAsync(ct);
        return tags.Select(t => new ConceptTagDto(t.Id, t.NameTr, t.NameEn, t.IsSystem, t.IsActive));
    }
}
