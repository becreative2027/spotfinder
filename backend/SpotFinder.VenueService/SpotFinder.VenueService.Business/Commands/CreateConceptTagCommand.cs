using FluentValidation;
using MediatR;
using SpotFinder.VenueService.Business.Models;
using SpotFinder.VenueService.Data.Entities;
using SpotFinder.VenueService.Data.Repositories;

namespace SpotFinder.VenueService.Business.Commands;

public record CreateConceptTagCommand(string NameTr, string NameEn) : IRequest<ConceptTagDto>;

public class CreateConceptTagCommandHandler : IRequestHandler<CreateConceptTagCommand, ConceptTagDto>
{
    private readonly IConceptTagRepository _repo;
    public CreateConceptTagCommandHandler(IConceptTagRepository repo) => _repo = repo;

    public async Task<ConceptTagDto> Handle(CreateConceptTagCommand request, CancellationToken ct)
    {
        var tag = new ConceptTag
        {
            NameTr = request.NameTr,
            NameEn = request.NameEn,
            IsSystem = false,
            IsActive = true
        };
        await _repo.AddAsync(tag, ct);
        await _repo.SaveChangesAsync(ct);
        return new ConceptTagDto(tag.Id, tag.NameTr, tag.NameEn, tag.IsSystem, tag.IsActive);
    }
}

public class CreateConceptTagCommandValidator : AbstractValidator<CreateConceptTagCommand>
{
    public CreateConceptTagCommandValidator()
    {
        RuleFor(c => c.NameTr).NotEmpty().MaximumLength(100);
        RuleFor(c => c.NameEn).NotEmpty().MaximumLength(100);
    }
}
