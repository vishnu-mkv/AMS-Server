using AMS.Models;
using AMS.Requests;

namespace AMS.Interfaces;

public interface IScheduleManager
{

    bool CheckIfScheduleExists(string id, string organizationId);
    Schedule AddSchedule(AddScheduleRequest addScheduleRequest);
    Schedule? GetSchedule(string id);
    List<Schedule> GetSchedules();
}