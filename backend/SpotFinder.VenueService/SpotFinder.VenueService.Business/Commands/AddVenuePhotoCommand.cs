using FluentValidation;
using MediatR;
using SpotFinder.VenueService.Business.Models;
using SpotFinder.VenueService.Data.Entities;
using SpotFinder.VenueService.Data.Repositories;

namespace SpotFinder.VenueService.Business.Commands;

public record AddVenuePhotoCommand(
    Guid VenueId,
    string Url,
    bool IsMenuPhoto,
    int DisplayOrder
) : IRequest<VenuePhotoDto>;

public class AddVenuePhotoCommandHandler : IRequestHandler<AddVenuePhotoCommand, VenuePhotoDto>
{
    private readonly IVenueRepository _venueRepo;
    private readonly IVenuePhotoRepository _photoRepo;

    public AddVenuePhotoCommandHandler(IVenueRepository venueRepo, IVenuePhotoRepository photoRepo)
    {
        _venueRepo = venueRepo;
        _photoRepo = photoRepo;
    }

    public async Task<VenuePhotoDto> Handle(AddVenuePhotoCommand request, CancellationToken ct)
    {
        var venue = await _venueRepo.GetByIdAsync(request.VenueId, ct)
            ?? throw new KeyNotFoundException("Mekân bulunamadı.");

        var photo = new VenuePhoto
        {
            VenueId = venue.Id,
            Url = request.Url,
            IsMenuPhoto = request.IsMenuPhoto,
            DisplayOrder = request.DisplayOrder,
            CreatedAt = DateTime.UtcNow
        };

        await _photoRepo.AddAsync(photo, ct);
        await _photoRepo.SaveChangesAsync(ct);

        return new VenuePhotoDto(photo.Id, photo.Url, photo.IsMenuPhoto, photo.DisplayOrder);
    }
}

public class AddVenuePhotoCommandValidator : AbstractValidator<AddVenuePhotoCommand>
{
    public AddVenuePhotoCommandValidator()
    {
        RuleFor(c => c.VenueId).NotEmpty();
        RuleFor(c => c.Url).NotEmpty().MaximumLength(500);
    }
}
