using AMS.Data;
using AMS.Interfaces;
using AMS.Models;
using AMS.Requests;
using Microsoft.EntityFrameworkCore;

namespace AMS.Managers;

public class SessionManager : ISessionManager
{
    private readonly ApplicationDbContext _context;
    private readonly ISlotManager _slotManager;
    private readonly IGroupManager _groupManager;

    public SessionManager(ApplicationDbContext context, ISlotManager slotManager, IGroupManager groupManager)
    {
        _context = context;
        _slotManager = slotManager;
        _groupManager = groupManager;
    }

    private void GroupScheduleCheck(string scheduleId, string[] groupIds)
    {
        // check if all groups have the same schedule
        if (!_groupManager.CheckIfAllGroupsHaveSameSchedule(scheduleId, groupIds))
        {
            throw new Exception("All groups must have the same schedule.");
        }


        var groupsWithNoSchedule = _context.Groups.Where(g => g.ScheduleId == null && groupIds.Contains(g.Id)).ToList();

        _groupManager.PopulateScheduleId(groupsWithNoSchedule, scheduleId);

    }

    public Session AddSession(AddSessionRequest addSessionRequest, string scheduleId)
    {

        GroupScheduleCheck(scheduleId, addSessionRequest.GroupIds);

        Session session = new()
        {
            ScheduleId = scheduleId,
            Groups = _context.Groups.Where(g => addSessionRequest.GroupIds.Contains(g.Id)).ToList(),
            AttendanceTakers = _context.Users.Where(u => addSessionRequest.AttendanceTakerIds.Contains(u.Id)).ToList(),
            Slots = _slotManager.EnsureSlots(scheduleId, addSessionRequest.Slots)
        };

        if (addSessionRequest.TopicId != null)
        {
            session.TopicId = addSessionRequest.TopicId;
        }

        _context.Sessions.Add(session);
        _context.SaveChanges();
        return session;
    }

    public bool CheckIfSessionExists(string id, string scheduleId)
    {
        return CheckIfSessionsExist(new string[] { id }, scheduleId);
    }

    public bool CheckIfSessionsExist(string[] ids, string scheduleId)
    {
        // check if all sessions exist
        return _context.Sessions.Where(s => ids.Contains(s.Id) && s.ScheduleId == scheduleId).Count() == ids.Length;
    }

    public bool CleanSessionsForDay(string scheduleId, int day)
    {
        // delete slots for day
        var sessions = _context.Slots.Where(s => s.ScheduleId == scheduleId && s.Day == day).ToList();
        _context.Slots.RemoveRange(sessions);

        _context.SaveChanges();

        // delete sessions for with no slots
        var sessionsWithNoSlots = _context.Sessions.Include(s => s.Slots).Where(s => s.ScheduleId == scheduleId && s.Slots.Count == 0).ToList();
        _context.Sessions.RemoveRange(sessionsWithNoSlots);

        return true;
    }

    public bool DeleteSession(string id)
    {
        _context.Sessions.Remove(GetSession(id) ?? throw new Exception("Session not found."));
        _context.SaveChanges();
        return true;
    }

    public Session? GetSession(string id, bool populate = false)
    {
        if (populate)
        {
            return _context.Sessions
                .Include(s => s.Groups)
                .Include(s => s.AttendanceTakers)
                .Include(s => s.Slots)
                .Include(s => s.Topic)
                .FirstOrDefault(s => s.Id == id);
        }
        return _context.Sessions.FirstOrDefault(s => s.Id == id);
    }

    public List<Session> GetSessions(string scheduleId)
    {
        return _context.Sessions.Where(s => s.ScheduleId == scheduleId).ToList();
    }

    public Session UpdateSession(string id, AddSessionRequest updateSessionRequest)
    {
        Session session = _context.Sessions.Include(s => s.Slots).Include(s => s.Groups).Include(s => s.AttendanceTakers).FirstOrDefault(s => s.Id == id &&
            s.ScheduleId == updateSessionRequest.ScheduleId
        ) ?? throw new Exception("Session not found.");

        GroupScheduleCheck(updateSessionRequest.ScheduleId, updateSessionRequest.GroupIds);

        var AllSlots = _slotManager.EnsureSlots(session.ScheduleId, updateSessionRequest.Slots);
        session.Slots = AllSlots;

        // update groups
        if (updateSessionRequest.GroupIds != null)
        {
            var newGroups = _context.Groups.Where(g => updateSessionRequest.GroupIds.Contains(g.Id)).ToList();
            if (newGroups.Count != updateSessionRequest.GroupIds.Length)
            {
                throw new Exception("Group not found.");
            }

            session.Groups = newGroups;
        }


        // add attendance takers that are not already in the session
        if (updateSessionRequest.AttendanceTakerIds != null)
        {

            var newAttendanceTakers = _context.Users.Where(u => updateSessionRequest.AttendanceTakerIds.Contains(u.Id)).ToList();
            if (newAttendanceTakers.Count != updateSessionRequest.AttendanceTakerIds.Length)
            {
                throw new Exception("Attendance taker not found.");
            }

            session.AttendanceTakers = newAttendanceTakers;
        }


        session.TopicId = updateSessionRequest.TopicId;

        _context.Sessions.Update(session);
        _context.SaveChanges();
        return session;
    }
}
