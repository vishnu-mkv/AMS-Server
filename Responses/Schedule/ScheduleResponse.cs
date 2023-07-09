namespace AMS.Responses
{
    public class ScheduleResponse
    {

        public string Id { get; set; }

        public string Name { get; set; }

        public string Color { get; set; }

        public int[] Days { get; set; }
    }

    public class ScheduleDetailResponse : ScheduleResponse
    {

        public TimeSlotResponse[] TimeSlots { get; set; }

        public SessionDetailResponse[] Sessions { get; set; }
    }
}