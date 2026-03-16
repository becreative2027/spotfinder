using Microsoft.EntityFrameworkCore;
using SpotFinder.SearchService.Data.Context;
using SpotFinder.SearchService.Data.Entities;

namespace SpotFinder.SearchService.Data.Repositories;

public class SearchRepository : ISearchRepository
{
    private readonly AppDbContext _db;

    public SearchRepository(AppDbContext db) => _db = db;

    public async Task<(IEnumerable<VenueReadModel> Items, int TotalCount)> SearchAsync(
        int? districtId,
        IEnumerable<int>? conceptTagIds,
        string sortBy,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _db.Venues
            .Include(v => v.District)
            .Include(v => v.Photos.OrderBy(p => p.DisplayOrder))
            .Include(v => v.VenueConcepts)
                .ThenInclude(vc => vc.ConceptTag)
            .Where(v => v.IsActive);

        if (districtId.HasValue)
            query = query.Where(v => v.DistrictId == districtId.Value);

        var tagIds = conceptTagIds?.ToList();
        if (tagIds is { Count: > 0 })
            query = query.Where(v => tagIds.All(tagId => v.VenueConcepts.Any(vc => vc.ConceptTagId == tagId)));

        var totalCount = await query.CountAsync(ct);

        query = sortBy == "newest"
            ? query.OrderByDescending(v => v.CreatedAt)
            : query.OrderByDescending(v => v.AverageRating).ThenByDescending(v => v.CreatedAt);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<IEnumerable<VenueReadModel>> GetFeaturedAsync(int count = 10, CancellationToken ct = default)
    {
        return await _db.Venues
            .Include(v => v.District)
            .Include(v => v.Photos.OrderBy(p => p.DisplayOrder))
            .Include(v => v.VenueConcepts)
                .ThenInclude(vc => vc.ConceptTag)
            .Where(v => v.IsActive)
            .OrderByDescending(v => v.AverageRating)
            .ThenByDescending(v => v.ReviewCount)
            .ThenByDescending(v => v.CreatedAt)
            .Take(count)
            .ToListAsync(ct);
    }
}
