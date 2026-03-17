using SpotFinder.VenueService.Data.Entities;

namespace SpotFinder.VenueService.Data.Repositories;

public interface IConceptTagRepository
{
    Task<IEnumerable<ConceptTag>> GetAllActiveAsync(CancellationToken ct = default);
    Task<ConceptTag?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(ConceptTag tag, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
