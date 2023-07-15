using AMS.Models;

namespace AMS.Responses;

public class GroupSummaryResponse
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Color { get; set; }
    public GroupType GroupType { get; set; }
    public bool Disabled { get; set; }
    public string ScheduleId { get; set; } = null!;
}

public class GroupResponse : GroupSummaryResponse
{
    public ScheduleResponse? Schedule { get; set; }
    public List<UserBaseSummaryResponse> Users { get; set; } = null!;
    public List<GroupSummaryResponse> Groups { get; set; } = null!;

}