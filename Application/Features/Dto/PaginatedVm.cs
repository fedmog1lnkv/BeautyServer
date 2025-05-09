namespace Application.Features.Dto;

public sealed class PaginatedVm<T>
{
    public List<T> Data { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => 
        (int)Math.Ceiling((double)TotalCount / PageSize);
}