namespace AMS.Responses;

public class SlotResponse
{

    public string Id { get; set; }

    public string TimeSlotId { get; set; }
    public TimeSlotResponse TimeSlot { get; set; }

    public int Day { get; set; }
}