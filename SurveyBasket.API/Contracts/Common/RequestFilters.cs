namespace SurveyBasket.API.Contracts.Common;

public record RequestFilters
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10; // validate on if the pagesize max value 
    public string? SearchValue { get; init; } // filtering to search 

    public string? SortColumn { get; init; }
    public string? SortDirection { get; init; } = "ASC"; // for Sorting 

}
