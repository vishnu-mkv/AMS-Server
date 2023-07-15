using AMS.DTO;
using AMS.Models;
using AMS.Requests;
using AMS.Responses;

namespace AMS.Interfaces;

public interface IAttendanceManager
{

    public Attendance? GetAttendance(string attendanceId, bool populate = false);

    public bool AttendanceExists(string attendanceId);

    public Attendance AddAttendance(AddAttendanceRequest request);

    public Attendance UpdateAttendance(UpdateAttendanceRequest request);

    public bool CheckIfUserHasPermission(string[] GroupAccessPath, string targetGroupId, string sessionId);

    public PaginationDTO<Attendance> ListAttendances(AttendancePaginationQuery paginationQuery);
    void DeleteAttendance(string attendanceId);

    public GroupReportView GetGroupReport(AttendanceReportRequest request);


}