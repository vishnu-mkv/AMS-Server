using AMS.Models;
using AMS.Requests;

namespace AMS.Interfaces;

public interface IScheduleManager
{

    bool CheckIfScheduleExists(string id, string organizationId);
    bool CheckIfScheduleExists(string id);
    Schedule AddSchedule(AddScheduleRequest addScheduleRequest);
    Schedule? GetSchedule(string id, bool populate = false);
    List<Schedule> GetSchedules();
    Schedule? UpdateSchedule(string id, UpdateScheduleRequest updateScheduleRequest);

}