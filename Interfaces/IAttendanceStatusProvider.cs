
using AMS.Models;

namespace AMS.Interfaces;

public interface IAttendanceStatusProvider
{
    List<AttendanceStatus?> GetAttendanceStatus(string[] attendanceStatusIds);
    AttendanceStatus? GetAttendanceStatus(string attendanceStatusId);
    bool AttendanceStatusExists(string attendanceStatusId);
    bool AttendanceStatusExists(string[] attendanceStatusId);
    List<AttendanceStatus> GetAllStatus();
}