namespace Application.Features.Records.Queries.GetRecordCommentsByEntityId.Dto;

public sealed class RecordReviewVm
{
    public Guid RecordId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}