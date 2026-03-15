using FluentValidation;
using MediatR;
using SpotFinder.VenueService.Business.Models;
using SpotFinder.VenueService.Data.Entities;
using SpotFinder.VenueService.Data.Repositories;

namespace SpotFinder.VenueService.Business.Commands;

public record CreateVenueCommand(
    string Name,
    string? Description,
    int? DistrictId,
    string? Address,
    string? ParkingStatus,
    decimal? Lat,
    decimal? Lng,
    IEnumerable<int>? ConceptTagIds
) : IRequest<VenueDto>;

public class CreateVenueCommandHandler : IRequestHandler<CreateVenueCommand, VenueDto>
{
    private readonly IVenueRepository _venueRepo;
    private readonly IDistrictRepository _districtRepo;
    private readonly IConceptTagRepository _conceptTagRepo;

    public CreateVenueCommandHandler(
        IVenueRepository venueRepo,
        IDistrictRepository districtRepo,
        IConceptTagRepository conceptTagRepo)
    {
        _venueRepo = venueRepo;
        _districtRepo = districtRepo;
        _conceptTagRepo = conceptTagRepo;
    }

    public async Task<VenueDto> Handle(CreateVenueCommand request, CancellationToken ct)
    {
        var venue = new Venue
        {
            Name = request.Name,
            Description = request.Description,
            DistrictId = request.DistrictId,
            Address = request.Address,
            ParkingStatus = request.ParkingStatus,
            Lat = request.Lat,
            Lng = request.Lng
        };

        if (request.ConceptTagIds?.Any() == true)
        {
            foreach (var tagId in request.ConceptTagIds.Distinct())
            {
                var tag = await _conceptTagRepo.GetByIdAsync(tagId, ct);
                if (tag is not null)
                    venue.VenueConcepts.Add(new VenueConcept { VenueId = venue.Id, ConceptTagId = tagId });
            }
        }

        await _venueRepo.AddAsync(venue, ct);
        await _venueRepo.SaveChangesAsync(ct);

        var saved = await _venueRepo.GetByIdAsync(venue.Id, ct);
        return new VenueDto(
            saved!.Id, saved.Name, saved.Description,
            saved.DistrictId, saved.District?.Name,
            saved.Address, saved.ParkingStatus,
            saved.Lat, saved.Lng,
            saved.AverageRating, saved.ReviewCount, saved.IsActive, saved.CreatedAt,
            saved.Photos.Select(p => new VenuePhotoDto(p.Id, p.Url, p.IsMenuPhoto, p.DisplayOrder)),
            saved.VenueConcepts.Select(vc => new ConceptTagDto(vc.ConceptTag.Id, vc.ConceptTag.NameTr, vc.ConceptTag.NameEn, vc.ConceptTag.IsSystem, vc.ConceptTag.IsActive))
        );
    }
}

public class CreateVenueCommandValidator : AbstractValidator<CreateVenueCommand>
{
    public CreateVenueCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(200);
        RuleFor(c => c.ParkingStatus)
            .Must(v => v is null || new[] { "available", "unavailable", "valet" }.Contains(v))
            .WithMessage("ParkingStatus must be 'available', 'unavailable', or 'valet'");
    }
}
