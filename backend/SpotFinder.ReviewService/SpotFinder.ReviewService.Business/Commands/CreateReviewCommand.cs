using FluentValidation;
using MediatR;
using SpotFinder.ReviewService.Business.Models;
using SpotFinder.ReviewService.Data.Entities;
using SpotFinder.ReviewService.Data.Repositories;

namespace SpotFinder.ReviewService.Business.Commands;

public record CreateReviewCommand(Guid VenueId, Guid UserId, string? Body, short Rating) : IRequest<ReviewDto>;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, ReviewDto>
{
    private readonly IReviewRepository _repo;

    public CreateReviewCommandHandler(IReviewRepository repo) => _repo = repo;

    public async Task<ReviewDto> Handle(CreateReviewCommand request, CancellationToken ct)
    {
        var review = new Review
        {
            Id = Guid.NewGuid(),
            VenueId = request.VenueId,
            UserId = request.UserId,
            Body = request.Body,
            Rating = request.Rating,
            Status = "pending",
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(review, ct);
        await _repo.SaveChangesAsync(ct);

        return new ReviewDto(review.Id, review.VenueId, review.UserId, review.Body,
            review.Rating, review.Status, review.CreatedAt, review.UpdatedAt);
    }
}

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.VenueId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween((short)1, (short)5)
            .WithMessage("Rating must be between 1 and 5.");
        RuleFor(x => x.Body).MaximumLength(2000).When(x => x.Body != null);
    }
}
