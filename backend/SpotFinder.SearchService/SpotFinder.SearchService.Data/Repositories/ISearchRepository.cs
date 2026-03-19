using SpotFinder.SearchService.Data.Entities;

namespace SpotFinder.SearchService.Data.Repositories;

public interface ISearchRepository
{
    Task<(IEnumerable<VenueReadModel> Items, int TotalCount)> SearchAsync(
        int? districtId,
        IEnumerable<int>? conceptTagIds,
        string sortBy,
        int page,
        int pageSize,
        string? nameQuery = null,
        CancellationToken ct = default);

    Task<IEnumerable<VenueReadModel>> GetFeaturedAsync(int count = 10, CancellationToken ct = default);
}
