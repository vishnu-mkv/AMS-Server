using AMS.Models;
using AMS.Requests;

namespace AMS.Interfaces;

public interface ISessionManager
{
    public Session AddSession(AddSessionRequest addSessionRequest, string scheduleId);
    public Session UpdateSession(string id, AddSessionRequest updateSessionRequest);
    public bool CheckIfSessionExists(string id, string scheduleId);
    public bool CheckIfSessionsExist(string[] ids, string scheduleId);
    public Session? GetSession(string id, bool populate = false);
    public bool CleanSessionsForDay(string scheduleId, int day);
    public List<Session> GetSessions(string scheduleId);
    public bool DeleteSession(string id);
}