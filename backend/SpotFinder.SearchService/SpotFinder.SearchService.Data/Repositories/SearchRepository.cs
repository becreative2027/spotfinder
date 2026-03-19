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
        string? nameQuery = null,
        CancellationToken ct = default)
    {
        // Base filtering — no includes yet, no ordering
        IQueryable<VenueReadModel> baseQuery = _db.Venues.Where(v => v.IsActive);

        if (districtId.HasValue)
            baseQuery = baseQuery.Where(v => v.DistrictId == districtId.Value);

        if (!string.IsNullOrWhiteSpace(nameQuery))
        {
            var lower = nameQuery.Trim().ToLower();
            baseQuery = baseQuery.Where(v => v.Name.ToLower().Contains(lower));
        }

        var tagIds = conceptTagIds?.ToList();
        if (tagIds is { Count: > 0 })
        {
            // OR semantics: venue must match at least one selected tag
            baseQuery = baseQuery.Where(v => v.VenueConcepts.Any(vc => tagIds.Contains(vc.ConceptTagId)));
        }

        var totalCount = await baseQuery.CountAsync(ct);

        // Ordering — venues matching more tags rank first (AND venues top, OR-only below)
        IOrderedQueryable<VenueReadModel> orderedQuery;
        if (tagIds is { Count: > 0 })
        {
            orderedQuery = sortBy == "newest"
                ? baseQuery
                    .OrderByDescending(v => v.VenueConcepts.Count(vc => tagIds.Contains(vc.ConceptTagId)))
                    .ThenByDescending(v => v.CreatedAt)
                : baseQuery
                    .OrderByDescending(v => v.VenueConcepts.Count(vc => tagIds.Contains(vc.ConceptTagId)))
                    .ThenByDescending(v => v.AverageRating)
                    .ThenByDescending(v => v.CreatedAt);
        }
        else
        {
            orderedQuery = sortBy == "newest"
                ? baseQuery.OrderByDescending(v => v.CreatedAt)
                : baseQuery.OrderByDescending(v => v.AverageRating).ThenByDescending(v => v.CreatedAt);
        }

        // Get ordered IDs for the page
        var ids = await orderedQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(v => v.Id)
            .ToListAsync(ct);

        // Fetch full data with includes for only the page items
        var venues = await _db.Venues
            .Include(v => v.District)
            .Include(v => v.Photos.OrderBy(p => p.DisplayOrder))
            .Include(v => v.VenueConcepts)
                .ThenInclude(vc => vc.ConceptTag)
            .Where(v => ids.Contains(v.Id))
            .ToListAsync(ct);

        // Restore the ordered sequence
        var items = ids.Select(id => venues.First(v => v.Id == id));

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
