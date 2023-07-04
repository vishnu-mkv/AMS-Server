namespace AMS.Responses;

public class SessionSummaryResponse
{

    public string Id { get; set; }

    public TopicResponse Topic { get; set; }
    public string TopicId { get; set; }

}

public class SessionResponse : SessionSummaryResponse
{

    public List<SlotResponse> slots { get; set; }
}

public class SessionDetailResponse : SessionResponse
{

    public List<GroupSummaryResponse> Groups { get; set; }

    public List<UserSummaryResponse> AttendanceTakers { get; set; }
}