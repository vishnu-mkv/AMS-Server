using AMS.Models;

namespace AMS.DTO;

public class PaginationDTO<T>
{
    public int Page { get; set; }
    public int Limit { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;

    public T[] Docs { get; set; } = Array.Empty<T>();

    public PaginationDTO() { }


    public PaginationDTO(IQueryable<T> query, PaginationQuery paginationQuery)
    {
        var count = query.Count();
        var items = query.Skip((paginationQuery.Page - 1) * paginationQuery.Limit)
            .Take(paginationQuery.Limit)
            .ToArray();

        Page = paginationQuery.Page;
        Limit = paginationQuery.Limit;
        TotalCount = count;
        TotalPages = (int)Math.Ceiling(count / (double)paginationQuery.Limit);
        Docs = items;

    }
}

public class PaginationQuery
{
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;
    public string Search { get; set; } = "";
    // sort by
    public string SortBy { get; set; } = "createdAt";
    public string Order { get; set; } = "desc";
}

public class UserPaginationQuery : PaginationQuery
{
    public string[] Roles { get; set; } = Array.Empty<string>();
}

public class GroupPaginationQuery : PaginationQuery
{
    // scheduleId

    public string[]? ScheduleId { get; set; } = null;
    // group type
    public GroupType? GroupType { get; set; } = null;
}


public class AttendancePaginationQuery : PaginationQuery
{
    public string[] SessionId { get; set; } = null;

    public string[] GroupId { get; set; } = null;

    public DateTime? RecordedForDate { get; set; } = null;

    public string[] TimeSlotId { get; set; } = null;

    public string? ScheduleId { get; set; } = null;

    public string? AttendanceTakerId { get; set; } = null;

    public DateTime? StartDate { get; set; } = null;

    public DateTime? EndDate { get; set; } = null;
}