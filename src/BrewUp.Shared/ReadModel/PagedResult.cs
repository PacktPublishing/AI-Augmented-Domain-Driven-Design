namespace BrewUp.Shared.ReadModel;

public class PagedResult<T>(IEnumerable<T> results, int page, int pageSize, int totalRecords)   
{
    public IEnumerable<T> Results { get; } = results;
    public int PageSize { get; } = pageSize;
    public int Page { get; } = page;
    public int TotalRecords { get; } = totalRecords;
}