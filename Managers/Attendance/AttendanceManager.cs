using AMS.Data;
using AMS.DTO;
using AMS.Interfaces;
using AMS.Models;
using AMS.Requests;
using AMS.Responses;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AMS.Managers;

public class AttendanceManager : IAttendanceManager
{

    private readonly IAuthManager authManager;
    private readonly ApplicationDbContext context;
    private readonly ISlotManager slotManager;
    private readonly IMapper mapper;

    public AttendanceManager(ApplicationDbContext context, IAuthManager authManager, ISlotManager slotManager, IMapper mapper)
    {
        this.context = context;
        this.authManager = authManager;
        this.slotManager = slotManager;
        this.mapper = mapper;
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

        var session = context.Sessions.FirstOrDefault(x => x.Id == request.SessionId);

        var Id = Guid.NewGuid().ToString();
        var attendance = new Attendance
        {
            Id = Id,
            GroupId = request.GroupId,
            ScheduleId = request.ScheduleId,
            SlotId = Slot.Id,
            SessionId = request.SessionId,
            RecordedFor = request.Date,
            TopicId = session?.TopicId,
            Records = request.AttendanceEntries.Select(x => new Record
            {
                AttendanceId = Id,
                UserId = x.UserId,
                AttendanceStatusId = x.AttendanceStatusId
            }).ToList(),
            OrganizationId = authManager.GetUserOrganizationId()
        };

        context.Attendances.Add(attendance);
        context.SaveChanges();

        return attendance;
    }

    public bool AttendanceExists(string attendanceId)
    {
        return context.Attendances.Any(x => x.Id == attendanceId && x.OrganizationId == authManager.GetUserOrganizationId());
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
            return context.Attendances.Include(a => a.Group).Include(s => s.Topic)
                .Include(a => a.Schedule).Include(a => a.Slot).ThenInclude(s => s.TimeSlot)
                .Include(a => a.Records).ThenInclude(r => r.User)
                .FirstOrDefault(x => x.Id == attendanceId && x.OrganizationId == authManager.GetUserOrganizationId());
        }
        else
        {
            return context.Attendances.FirstOrDefault(x => x.Id == attendanceId && x.OrganizationId == authManager.GetUserOrganizationId());
        }
    }

    public PaginationDTO<Attendance> ListAttendances(AttendancePaginationQuery paginationQuery)
    {
        var query = context.Attendances.Include(x => x.Group)
            .Include(x => x.Topic).Include(x => x.Slot).ThenInclude(x => x.TimeSlot)
            .Include(x => x.Schedule)
            .Where(x => x.OrganizationId == authManager.GetUserOrganizationId())
            .AsQueryable();

        if (paginationQuery.TopicId != null)
        {
            query = query.Where(x => paginationQuery.TopicId.Contains(x.TopicId));
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

    public GroupReportView GetGroupReport(AttendanceReportRequest request)
    {
        string groupId = request.GroupId;

        var group = context.Groups.Include(x => x.Users).Include(x => x.Groups).FirstOrDefault(x => x.Id == groupId
            && x.OrganizationId == authManager.GetUserOrganizationId()
        ) ?? throw new Exception("Group not found");

        if (request.isUserReport == true)
        {
            return GetGroupOfUsersReport(group, request);
        }

        return GetGroupOfGroupsReport(group, request);
    }


    public GroupReportView GetGroupOfGroupsReport(Group group, AttendanceReportRequest request)
    {

        var userIds = group.Users.Select(x => x.Id).ToList();

        var records = context.Records.Include(x => x.Attendance).ThenInclude(x => x.Slot).ThenInclude(x => x.TimeSlot)
            .Include(x => x.Attendance).ThenInclude(x => x.Topic)
            .Where(x => x.UserId != null && userIds.Contains(x.UserId)).AsQueryable();

        if (request.StartDate != null)
        {
            records = records.Where(x => x.Attendance.RecordedFor.Date >= ((DateTime)request.StartDate).Date);
        }

        if (request.EndDate != null)
        {
            records = records.Where(x => x.Attendance.RecordedFor.Date <= ((DateTime)request.EndDate).Date);
        }

        if (request.TimeSlotIds != null)
        {
            records = records.Where(x => x.Attendance.Slot != null && request.TimeSlotIds.Contains(x.Attendance.Slot.TimeSlotId));
        }

        if (request.TopicIds != null)
        {
            records = records.Where(x => request.TopicIds.Contains(x.Attendance.TopicId));
        }

        if (request.AttendanceStatusIds != null)
        {
            records = records.Where(x => request.AttendanceStatusIds.Contains(x.AttendanceStatusId));
        }


        var timeSlotsQuery = context.TimeSlots.Where(x => x.ScheduleId == group.ScheduleId).AsQueryable();

        if (request.TimeSlotIds != null)
        {
            timeSlotsQuery = timeSlotsQuery.Where(x => request.TimeSlotIds.Contains(x.Id));
        }

        var query = records.AsEnumerable();

        if (request.Days != null)
        {
            query = records.AsEnumerable().Where(x => request.Days.Contains(x.Attendance.RecordedFor.DayOfWeek));
        }
        var timeSlots = timeSlotsQuery.ToList();

        var attendanceRecords = query.GroupBy(x => new
        {
            x.Attendance.RecordedFor.Date,
            x.Attendance.Slot.TimeSlotId,
            x.AttendanceStatusId
        }).OrderBy(x => x.Key.Date)
        .ThenBy(x => x.Key.AttendanceStatusId)
        .Select(x => new
        {
            x.Key.Date,
            x.Key.TimeSlotId,
            x.Key.AttendanceStatusId,
            Count = x.Count()
        }).ToList();

        // if not start date, or end date, find the earliest and latest dates
        // if no records, return empty list
        if (attendanceRecords.Count == 0)
        {
            return new GroupReportView
            {
                Group = mapper.Map<GroupResponse>(group),
                DayReports = new List<DayReportView>()
            };
        }


        List<DayReportView> dayReports = new();

        var AttendanceStatuses = context.AttendanceStatuses.Where(x => x.OrganizationId == group.OrganizationId).ToList();

        foreach (var record in attendanceRecords)
        {

            var dayReport = dayReports.FirstOrDefault(x => x.Date == record.Date);

            if (dayReport == null)
            {
                dayReport = new DayReportView
                {
                    Date = record.Date,
                    TimeSlotReports = new List<TimeSlotReportView>()
                };

                dayReports.Add(dayReport);
            }

            var timeSlotReport = dayReport.TimeSlotReports.FirstOrDefault(x => x.TimeSlotId == record.TimeSlotId);

            if (timeSlotReport == null)
            {
                timeSlotReport = new TimeSlotReportView
                {
                    TimeSlotId = record.TimeSlotId,
                    AttendanceStatusCounts = new List<AttendanceStatusCount>()
                };

                dayReport.TimeSlotReports.Add(timeSlotReport);
            }

            var attendanceStatusReport = timeSlotReport.AttendanceStatusCounts.FirstOrDefault(x => x.AttendanceStatusId == record.AttendanceStatusId);

            if (attendanceStatusReport == null)
            {
                attendanceStatusReport = new AttendanceStatusCount
                {
                    AttendanceStatusId = record.AttendanceStatusId,
                    Count = record.Count
                };

                timeSlotReport.AttendanceStatusCounts.Add(attendanceStatusReport);
            }
            else
            {

                attendanceStatusReport.Count += record.Count;
            }



        }

        if (group.GroupType == GroupType.GroupOfGroups)
        {
            group.Users = new List<ApplicationUser>();
        }

        return new GroupReportView
        {
            Group = mapper.Map<GroupResponse>(group),
            AttendanceStatuses = AttendanceStatuses,
            TimeSlots = mapper.Map<List<TimeSlotResponse>>(timeSlots),
            DayReports = dayReports
        };
    }

    private GroupReportView GetGroupOfUsersReport(Group group, AttendanceReportRequest request)
    {
        var userIds = group.Users.Select(x => x.Id).ToList();

        var records = context.Records.Include(x => x.Attendance).ThenInclude(x => x.Slot).ThenInclude(x => x.TimeSlot)
            .Include(x => x.Attendance).ThenInclude(x => x.Topic).Where(x => x.Attendance.GroupId == group.Id)
            .Where(x => x.UserId != null && userIds.Contains(x.UserId)).AsQueryable();

        if (request.StartDate != null)
        {
            records = records.Where(x => x.Attendance.RecordedFor.Date >= ((DateTime)request.StartDate).Date);
        }

        if (request.EndDate != null)
        {
            records = records.Where(x => x.Attendance.RecordedFor.Date <= ((DateTime)request.EndDate).Date);
        }

        if (request.TimeSlotIds != null)
        {
            records = records.Where(x => x.Attendance.Slot != null && request.TimeSlotIds.Contains(x.Attendance.Slot.TimeSlotId));
        }

        if (request.TopicIds != null)
        {
            records = records.Where(x => request.TopicIds.Contains(x.Attendance.TopicId));
        }

        if (request.AttendanceStatusIds != null)
        {
            records = records.Where(x => request.AttendanceStatusIds.Contains(x.AttendanceStatusId));
        }

        var timeSlotsQuery = context.TimeSlots.Where(x => x.ScheduleId == group.ScheduleId).AsQueryable();

        if (request.TimeSlotIds != null)
        {
            timeSlotsQuery = timeSlotsQuery.Where(x => request.TimeSlotIds.Contains(x.Id));
        }

        var recordDebug = records.ToList();

        var attendanceRecords = records
            .GroupBy(x => new { x.UserId, x.Attendance.RecordedFor.Date, x.Attendance.Slot.TimeSlotId, x.AttendanceStatusId })
            .OrderBy(x => x.Key.Date)
            .ThenBy(x => x.Key.AttendanceStatusId)
            .Select(x => new
            {
                x.Key.Date,
                x.Key.TimeSlotId,
                x.Key.AttendanceStatusId,
                x.Key.UserId,
                Count = x.Count()
            }).ToList();

        var userReports = group.Users.Select(x => new UserReportView
        {
            User = mapper.Map<UserSummaryResponse>(x),
            DayReports = new List<DayReportView>()
        }).ToList();

        if (attendanceRecords.Count == 0)
        {
            return new UserGroupReportView
            {
                Group = mapper.Map<GroupResponse>(group),
                DayReports = new List<DayReportView>(),
                UserReports = userReports
            };
        }

        var AttendanceStatuses = context.AttendanceStatuses.Where(x => x.OrganizationId == group.OrganizationId).ToList();

        foreach (var record in attendanceRecords)
        {


            var date = record.Date;
            var timeSlotId = record.TimeSlotId;
            var attendanceStatusId = record.AttendanceStatusId;
            var count = record.Count;

            var userRecord = userReports.FirstOrDefault(x => x.User.Id == record.UserId);

            var dayReports = userRecord.DayReports;

            var dayReport = dayReports.FirstOrDefault(x => x.Date == date);

            if (dayReport == null)
            {
                dayReport = new DayReportView
                {
                    Date = date,
                };

                dayReports.Add(dayReport);
            }

            var timeSlotReport = dayReport.TimeSlotReports.FirstOrDefault(x => x.TimeSlotId == timeSlotId);

            if (timeSlotReport == null)
            {
                timeSlotReport = new TimeSlotReportView
                {
                    TimeSlotId = timeSlotId,
                };

                dayReport.TimeSlotReports.Add(timeSlotReport);
            }

            timeSlotReport.AttendanceStatusCounts.Add(new AttendanceStatusCount
            {
                AttendanceStatusId = attendanceStatusId,
                Count = count
            });

        }

        return new UserGroupReportView
        {
            Group = mapper.Map<GroupResponse>(group),
            AttendanceStatuses = AttendanceStatuses,
            TimeSlots = mapper.Map<List<TimeSlotResponse>>(timeSlotsQuery.ToList()),
            DayReports = new List<DayReportView>(),
            UserReports = userReports
        };
    }
}
