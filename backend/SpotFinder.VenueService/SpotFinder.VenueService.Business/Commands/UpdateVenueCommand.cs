using FluentValidation;
using MediatR;
using SpotFinder.VenueService.Business.Models;
using SpotFinder.VenueService.Data.Entities;
using SpotFinder.VenueService.Data.Repositories;

namespace SpotFinder.VenueService.Business.Commands;

public record UpdateVenueCommand(
    Guid Id,
    string Name,
    string? Description,
    int? DistrictId,
    string? Address,
    string? ParkingStatus,
    decimal? Lat,
    decimal? Lng,
    IEnumerable<int>? ConceptTagIds
) : IRequest<VenueDto?>;

public class UpdateVenueCommandHandler : IRequestHandler<UpdateVenueCommand, VenueDto?>
{
    private readonly IVenueRepository _venueRepo;
    private readonly IConceptTagRepository _conceptTagRepo;

    public UpdateVenueCommandHandler(IVenueRepository venueRepo, IConceptTagRepository conceptTagRepo)
    {
        _venueRepo = venueRepo;
        _conceptTagRepo = conceptTagRepo;
    }

    public async Task<VenueDto?> Handle(UpdateVenueCommand request, CancellationToken ct)
    {
        var venue = await _venueRepo.GetByIdAsync(request.Id, ct);
        if (venue is null) return null;

        venue.Name = request.Name;
        venue.Description = request.Description;
        venue.DistrictId = request.DistrictId;
        venue.Address = request.Address;
        venue.ParkingStatus = request.ParkingStatus;
        venue.Lat = request.Lat;
        venue.Lng = request.Lng;
        venue.UpdatedAt = DateTime.UtcNow;

        // Replace concept tags
        venue.VenueConcepts.Clear();
        if (request.ConceptTagIds?.Any() == true)
        {
            foreach (var tagId in request.ConceptTagIds.Distinct())
            {
                var tag = await _conceptTagRepo.GetByIdAsync(tagId, ct);
                if (tag is not null)
                    venue.VenueConcepts.Add(new VenueConcept { VenueId = venue.Id, ConceptTagId = tagId });
            }
        }

        await _venueRepo.SaveChangesAsync(ct);

        var saved = await _venueRepo.GetByIdAsync(venue.Id, ct);
        return new VenueDto(
            saved!.Id, saved.Name, saved.Description,
            saved.DistrictId, saved.District?.Name,
            saved.Address, saved.ParkingStatus,
            saved.Lat, saved.Lng,
            saved.AverageRating, saved.ReviewCount, saved.IsActive, saved.CreatedAt,
            $"spotfinder://venue/{saved.Id}",
            saved.Photos.Select(p => new VenuePhotoDto(p.Id, p.Url, p.IsMenuPhoto, p.DisplayOrder)),
            saved.VenueConcepts.Select(vc => new ConceptTagDto(vc.ConceptTag.Id, vc.ConceptTag.NameTr, vc.ConceptTag.NameEn, vc.ConceptTag.IsSystem, vc.ConceptTag.IsActive))
        );
    }
}

public class UpdateVenueCommandValidator : AbstractValidator<UpdateVenueCommand>
{
    public UpdateVenueCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(200);
        RuleFor(c => c.ParkingStatus)
            .Must(v => v is null || new[] { "available", "unavailable", "valet" }.Contains(v))
            .WithMessage("ParkingStatus must be 'available', 'unavailable', or 'valet'");
    }
}
