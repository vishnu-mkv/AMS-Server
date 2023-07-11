using AMS.Data;
using AMS.DTO;
using AMS.Interfaces;
using AMS.Models;
using AMS.Requests;
using Microsoft.EntityFrameworkCore;

namespace AMS.Managers;

public class AttendanceManager : IAttendanceManager
{

    private readonly IAuthManager authManager;
    private readonly ApplicationDbContext context;
    private readonly ISlotManager slotManager;

    public AttendanceManager(ApplicationDbContext context, IAuthManager authManager, ISlotManager slotManager)
    {
        this.context = context;
        this.authManager = authManager;
        this.slotManager = slotManager;
    }

    public Attendance AddAttendance(AddAttendanceRequest request)
    {

        // check if attendance exists for the session, group, timeslot on the date

        if (context.Attendances.Include(x => x.Slot).Any(x => x.SessionId == request.SessionId && x.Slot.TimeSlotId == request.TimeSlotId
                                                    && x.GroupId == request.GroupId && x.RecordedFor == request.Date))
        {
            throw new Exception("Attendance already exists for this session, group, timeslot on this date.");
        }

        if (!CheckIfUserHasPermission(request.GroupAccessPath, request.GroupId, request.SessionId))
        {
            throw new UnauthorizedAccessException();
        }

        var Slot = slotManager.GetSlot(request.ScheduleId, (int)request.Date.DayOfWeek, request.TimeSlotId);

        var Id = Guid.NewGuid().ToString();
        var attendance = new Attendance
        {
            Id = Id,
            GroupId = request.GroupId,
            ScheduleId = request.ScheduleId,
            SlotId = Slot.Id,
            SessionId = request.SessionId,
            RecordedFor = request.Date,
            Records = request.AttendanceEntries.Select(x => new Record
            {
                AttendanceId = Id,
                UserId = x.UserId,
                AttendanceStatusId = x.AttendanceStatusId
            }).ToList()

        };

        context.Attendances.Add(attendance);
        context.SaveChanges();

        return attendance;
    }

    public bool AttendanceExists(string attendanceId)
    {
        return context.Attendances.Any(x => x.Id == attendanceId);
    }

    public bool CheckIfUserHasPermission(string[] GroupAccessPath, string targetGroupId, string sessionId)
    {
        // if last group in path is not target group, return false

        if (GroupAccessPath[^1] != targetGroupId)
        {
            return false;
        }

        var userId = authManager.GetCurrentUserId();


        var Session = context.Sessions.Include(x => x.AttendanceTakers).Include(x => x.Groups).FirstOrDefault(x => x.Id == sessionId);

        if (Session == null || !Session.Groups.Any(g => g.Id == GroupAccessPath[0]) || !Session.AttendanceTakers.Any(x => x.Id == userId))
        {
            return false;
        }
        // traverse from first to last group in the path
        // at each step, the next group must be a child of the current group
        // if not, return false


        for (int i = 0; i < GroupAccessPath.Length - 1; i++)
        {
            var currentGroupId = GroupAccessPath[i];
            var nextGroupId = GroupAccessPath[i + 1];

            var currentGroup = context.Groups.Include(x => x.Groups).FirstOrDefault(x => x.Id == currentGroupId);

            if (currentGroup == null || currentGroup.GroupType == GroupType.GroupOfUsers || !currentGroup.Groups.Any(x => x.Id == nextGroupId))
            {
                return false;
            }
        }

        // check if last group type is GroupOfUsers
        // if not, return false

        var lastGroup = context.Groups.FirstOrDefault(x => x.Id == targetGroupId);

        if (lastGroup == null || lastGroup.GroupType != GroupType.GroupOfUsers)
        {
            return false;
        }

        return true;
    }

    public void DeleteAttendance(string attendanceId)
    {
        var attendance = GetAttendance(attendanceId);

        if (attendance == null)
        {
            throw new KeyNotFoundException();
        }

        context.Attendances.Remove(attendance);
        context.SaveChanges();
    }

    public Attendance? GetAttendance(string attendanceId, bool populate = false)
    {
        if (populate)
        {
            return context.Attendances.Include(a => a.Group).Include(a => a.Session).ThenInclude(s => s.Topic)
                .Include(a => a.Schedule).Include(a => a.Slot).ThenInclude(s => s.TimeSlot)
                .Include(a => a.Records).ThenInclude(r => r.User)
                .FirstOrDefault(x => x.Id == attendanceId);
        }
        else
        {
            return context.Attendances.FirstOrDefault(x => x.Id == attendanceId);
        }
    }

    public PaginationDTO<Attendance> ListAttendances(AttendancePaginationQuery paginationQuery)
    {
        var query = context.Attendances.Include(x => x.Group).Include(x => x.Session)
            .ThenInclude(x => x.Topic).Include(x => x.Slot).ThenInclude(x => x.TimeSlot)
            .Include(x => x.Schedule).AsQueryable();

        if (paginationQuery.TopicId != null)
        {
            query = query.Where(x => x.Session != null && paginationQuery.TopicId.Contains(x.Session.TopicId));
        }

        if (paginationQuery.GroupId != null)
        {
            query = query.Where(x => paginationQuery.GroupId.Contains(x.GroupId));
        }

        if (paginationQuery.SessionId != null)
        {
            query = query.Where(x => paginationQuery.SessionId.Contains(x.SessionId));
        }

        if (paginationQuery.RecordedForDate != null)
        {
            query = query.Where(x => x.RecordedFor.Date == ((DateTime)paginationQuery.RecordedForDate).Date);
        }

        if (paginationQuery.ScheduleId != null)
        {
            query = query.Where(x => x.ScheduleId == paginationQuery.ScheduleId);
        }

        if (paginationQuery.TimeSlotId != null)
        {
            query = query.Include(x => x.Slot).Where(x => x.Slot != null && paginationQuery.TimeSlotId.Contains(x.Slot.TimeSlotId));
        }

        if (paginationQuery.AttendanceTakerId != null)
        {
            query = query.Include(x => x.Session).ThenInclude(x => x.AttendanceTakers).Where(x => x.Session.AttendanceTakers.Any(x => x.Id == paginationQuery.AttendanceTakerId));
        }

        if (paginationQuery.StartDate != null)
        {
            query = query.Where(x => x.RecordedFor.Date >= ((DateTime)paginationQuery.StartDate).Date);
        }

        if (paginationQuery.EndDate != null)
        {
            query = query.Where(x => x.RecordedFor.Date <= ((DateTime)paginationQuery.EndDate).Date);
        }

        // sort
        var sortDirection = paginationQuery.Order;
        query = sortDirection == "asc" ? query.OrderBy(x => x.RecordedFor) : query.OrderByDescending(x => x.RecordedFor);

        query = query.Include(a => a.Slot).Include(a => a.Group);

        return new PaginationDTO<Attendance>(query, paginationQuery);

    }

    public Attendance UpdateAttendance(UpdateAttendanceRequest request)
    {
        Attendance? attendance = GetAttendance(request.AttendanceId);

        if (attendance == null)
        {
            throw new KeyNotFoundException();
        }

        if (!CheckIfUserHasPermission(request.GroupAccessPath, attendance.GroupId, attendance.SessionId))
        {
            throw new UnauthorizedAccessException();
        }

        // update attendance records if they exist
        // add new records if they don't exist

        foreach (var entry in request.AttendanceEntries)
        {
            var record = context.Records.FirstOrDefault(x => x.AttendanceId == attendance.Id && x.UserId == entry.UserId);

            Console.WriteLine("record: " + record?.UserId + " " + record?.AttendanceStatusId);

            if (record == null)
            {
                // attendance.Records.Add(new Record
                // {
                //     AttendanceId = attendance.Id,
                //     UserId = entry.UserId,
                //     AttendanceStatusId = entry.AttendanceStatusId
                // });
            }
            else
            {
                record.AttendanceStatusId = entry.AttendanceStatusId;
            }
        }

        context.SaveChanges();
        return attendance;
    }
}
