using Microsoft.EntityFrameworkCore;
using SpotFinder.VenueService.Data.Context;
using SpotFinder.VenueService.Data.Entities;

namespace SpotFinder.VenueService.Data.Repositories;

public class ConceptTagRepository : IConceptTagRepository
{
    private readonly AppDbContext _db;
    public ConceptTagRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<ConceptTag>> GetAllActiveAsync(CancellationToken ct = default) =>
        await _db.ConceptTags.Where(t => t.IsActive).OrderBy(t => t.NameTr).ToListAsync(ct);

    public async Task<ConceptTag?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _db.ConceptTags.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task AddAsync(ConceptTag tag, CancellationToken ct = default) =>
        await _db.ConceptTags.AddAsync(tag, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _db.SaveChangesAsync(ct);
}
