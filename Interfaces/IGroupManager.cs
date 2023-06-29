using AMS.Requests;
using AMS.Models;
using AMS.DTO;

namespace AMS.Interfaces;

public interface IGroupManager
{

    public bool GroupExists(string organizationId, string groupId);
    public Group CreateGroup(AddGroupRequest request);
    public Group UpdateGroup(UpdateGroupRequest request, string groupId);
    public Group GetGroup(string groupId);
    public bool CheckGroupExists(string groupId);
    public bool CheckGroupExists(string groupId, string organizationId);
    public bool CheckIfAllGroupsHaveSameSchedule(string scheduleId, string[] groupId);
    public bool CheckIfAllUsersHaveSameSchedule(string scheduleId, string[] userId);

    public PaginationDTO<Group> GetGroups(GroupPaginationQuery paginationQuery);
}