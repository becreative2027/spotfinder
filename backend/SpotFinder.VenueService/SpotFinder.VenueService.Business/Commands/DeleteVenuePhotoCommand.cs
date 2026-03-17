using MediatR;
using SpotFinder.VenueService.Data.Repositories;

namespace SpotFinder.VenueService.Business.Commands;

public record DeleteVenuePhotoCommand(Guid VenueId, Guid PhotoId) : IRequest<bool>;

public class DeleteVenuePhotoCommandHandler : IRequestHandler<DeleteVenuePhotoCommand, bool>
{
    private readonly IVenuePhotoRepository _photoRepo;

    public DeleteVenuePhotoCommandHandler(IVenuePhotoRepository photoRepo)
    {
        _photoRepo = photoRepo;
    }

    public async Task<bool> Handle(DeleteVenuePhotoCommand request, CancellationToken ct)
    {
        var photo = await _photoRepo.GetByIdAsync(request.PhotoId, ct);
        if (photo is null || photo.VenueId != request.VenueId)
            return false;

        await _photoRepo.DeleteAsync(request.PhotoId, ct);
        await _photoRepo.SaveChangesAsync(ct);
        return true;
    }
}
