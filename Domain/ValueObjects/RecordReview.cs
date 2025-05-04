using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public sealed class RecordReview : ValueObject
{
    public const int MaxLength = 2000;
    public const int MinLength = 1;

    public const int MinRating = 1;
    public const int MaxRating = 10;

    private RecordReview(int rating, string? comment)
    {
        Rating = rating;
        Comment = comment;
    }

    public int Rating { get; }
    public string? Comment { get; }

    public static Result<RecordReview> Create(int rating, string? comment)
    {
        if (rating is < MinRating or > MaxRating)
            return Result.Failure<RecordReview>(DomainErrors.RecordReview.InvalidRating);


        comment = comment?.Trim();
        if (comment?.Length is < MinLength or > MaxLength)
            return Result.Failure<RecordReview>(DomainErrors.RecordReview.CommentLengthOutOfRange);

        return new RecordReview(rating, comment);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Rating;
        yield return Comment ?? string.Empty;
    }
}