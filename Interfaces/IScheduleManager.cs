using AMS.Models;
using AMS.Requests;

namespace AMS.Interfaces;

public interface IScheduleManager
{

    bool CheckIfScheduleExists(string id);
    Schedule AddSchedule(AddScheduleRequest addScheduleRequest);
    Schedule? GetSchedule(string id);
    List<Schedule> GetSchedules();
}