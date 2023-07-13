using AMS.Models;
using AMS.Data;
using AMS.Interfaces;
using AMS.Requests;
using Microsoft.EntityFrameworkCore;
using AMS.DTO;
using AMS.Migrations;

namespace AMS.Managers;

public class GroupManager : IGroupManager
{
    private readonly ApplicationDbContext context;
    private readonly IAuthManager authManager;

    public GroupManager(ApplicationDbContext context, IAuthManager authManager)
    {
        this.context = context;
        this.authManager = authManager;
    }

    public bool CheckIfAllGroupsHaveSameSchedule(string? scheduleId, string[] groupId)
    {
        // check if any of the groups does not have the same schedule id
        // if group schedule id is null, it means that it has the same schedule 
        // true if all groups have the same schedule id
        return !context.Groups.Any(w => groupId.Contains(w.Id) && ((w.ScheduleId != scheduleId && w.ScheduleId != null) || (w.ScheduleId != null && scheduleId == null)));
    }

    // given a list of users, if user schedule id is null, it means that it has the same schedule
    private void PopulateScheduleId(ICollection<ApplicationUser> users, string scheduleId)
    {
        foreach (var user in users)
        {
            if (user.ScheduleId == null)
            {
                user.ScheduleId = scheduleId;
            }
        }
    }

    public void PopulateScheduleId(ICollection<Group> groups, string scheduleId, int level = 10)
    {

        if (level == 0)
        {
            throw new Exception("Too many levels of groups");
        }

        foreach (var group in groups)
        {
            if (group.ScheduleId == null)
            {
                group.ScheduleId = scheduleId;
            }
            else
            {
                throw new Exception("Group already has a schedule");
            }

            var _group = context.Groups.Where(w => w.Id == group.Id).Include(i => i.Users).Include(i => i.Groups).FirstOrDefault();

            if (group.GroupType == GroupType.GroupOfUsers)
            {
                PopulateScheduleId(_group.Users, scheduleId);
            }
            else
            {
                PopulateScheduleId(_group.Groups, scheduleId, level - 1);
            }
        }
    }

    public bool CheckIfAllUsersHaveSameSchedule(string? scheduleId, string[] userId)
    {
        // check if any of the users does not have the same schedule id
        // if schedule id is null, then user should have null schedule id
        // if user schedule id is null, it means that it has the same schedule
        return !context.Users.Any(w => userId.Contains(w.Id) && ((w.ScheduleId != scheduleId && w.ScheduleId != null) || (w.ScheduleId != null && scheduleId == null)));
    }

    public bool CheckGroupExists(string groupId)
    {
        return context.Groups.Any(w => w.Id == groupId);
    }

    public bool CheckGroupExists(string groupId, string organizationId)
    {
        return context.Groups.Any(w => w.Id == groupId && w.OrganizationId == organizationId);
    }

    public Group CreateGroup(AddGroupRequest request)
    {
        Group group = new();

        if (request.Id is not null) group.Id = request.Id;
        group.Name = request.Name;
        group.GroupType = request.GroupType;
        group.ScheduleId = request.ScheduleId;
        group.CreatedById = authManager.GetCurrentUserId();
        group.OrganizationId = authManager.GetUserOrganizationId();

        if (request.Color != null)
            group.Color = request.Color;

        if (request.ScheduleId != null)
        {

            if (request.GroupType == GroupType.GroupOfUsers)
            {
                group.GroupType = GroupType.GroupOfUsers;

                if (request.Users != null)
                {
                    if (!CheckIfAllUsersHaveSameSchedule(request.ScheduleId, request.Users))
                    {
                        throw new InvalidOperationException("Users must have the same schedule");
                    }
                    group.Users = context.Users.Where(w => request.Users.Contains(w.Id)).Distinct().ToList();
                    PopulateScheduleId(group.Users, request.ScheduleId);
                }
            }
            else if (request.GroupType == GroupType.GroupOfGroups)
            {
                group.GroupType = GroupType.GroupOfGroups;

                if (request.Groups != null)
                {
                    if (!CheckIfAllGroupsHaveSameSchedule(request.ScheduleId, request.Groups))
                    {
                        throw new InvalidOperationException("Groups must have the same schedule");
                    }
                    group.Groups = context.Groups.Where(w => request.Groups.Contains(w.Id)).ToList();
                    PopulateScheduleId(group.Groups, request.ScheduleId);
                }
            }
        }

        context.Groups.Add(group);
        context.SaveChanges();

        return group;
    }

    public Group GetGroup(string groupId)
    {
        Group? group = context.Groups.Include(g => g.Users).Include(g => g.Groups).FirstOrDefault(w => w.Id == groupId);

        if (group is null || group.OrganizationId != authManager.GetUserOrganizationId())
        {
            throw new InvalidOperationException("Group does not exist");
        }

        return group;
    }

    public bool CheckIfUsersBelongToGroup(string groupId, string[] userIds)
    {
        // check if all users belong to the group
        return context.Users.Include(u => u.Groups).Count(w => userIds.Contains(w.Id) && w.Groups.Contains(new Group { Id = groupId })) == userIds.Length;
    }

    public PaginationDTO<Group> GetGroups(GroupPaginationQuery paginationQuery)
    {
        IQueryable<Group> queryable = context.Groups.Where(w => w.OrganizationId == authManager.GetUserOrganizationId());

        if (paginationQuery.Search is not null)
        {
            queryable = queryable.Where(w => w.Name.Contains(paginationQuery.Search));
        }

        if (paginationQuery.GroupType is not null)
        {
            queryable = queryable.Where(w => w.GroupType == paginationQuery.GroupType);
        }

        // sort order and sort by
        queryable = paginationQuery.Order switch
        {
            "asc" => paginationQuery.SortBy switch
            {
                "name" => queryable.OrderBy(w => w.Name),
                "createdBy" => queryable.OrderBy(w => w.CreatedBy),
                "createdAt" => queryable.OrderBy(w => w.CreatedAt),
                _ => queryable.OrderBy(w => w.Name)
            },
            "desc" => paginationQuery.SortBy switch
            {
                "name" => queryable.OrderByDescending(w => w.Name),
                "createdBy" => queryable.OrderByDescending(w => w.CreatedBy),
                "createdAt" => queryable.OrderByDescending(w => w.CreatedAt),
                _ => queryable.OrderByDescending(w => w.Name)
            },
            _ => queryable.OrderBy(w => w.Name)
        };

        // filter by schedule id
        queryable = paginationQuery.ScheduleId switch
        {
            null => queryable,
            _ => queryable.Where(w => paginationQuery.ScheduleId.Any(s => s == w.ScheduleId) || (w.ScheduleId == null && paginationQuery.ScheduleId.Contains("null")))
        };

        // pagination
        return new PaginationDTO<Group>(queryable, paginationQuery);
    }

    public bool GroupExists(string organizationId, string groupId)
    {
        return context.Groups.Any(w => w.OrganizationId == organizationId && w.Id == groupId);
    }

    public Group UpdateGroup(UpdateGroupRequest request, string id)
    {
        Group? group = context.Groups.Include(g => g.Users).Include(g => g.Groups).FirstOrDefault(w => w.Id == id);

        if (group is null || group.OrganizationId != authManager.GetUserOrganizationId())
        {
            throw new InvalidOperationException("Group does not exist");
        }

        if (request.Name != null)
        {
            group.Name = request.Name;
        }

        if (request.Color != null)
        {
            group.Color = request.Color;
        }

        if (request.Disabled != null)
        {
            group.Disabled = (bool)request.Disabled;
        }

        if (request.ScheduleId != null && group.ScheduleId == null)
        {
            group.ScheduleId = request.ScheduleId;
        }


        if (group.GroupType == GroupType.GroupOfUsers)
        {

            if (request.Users != null)
            {

                if (CheckIfAllUsersHaveSameSchedule(group.ScheduleId, request.Users) == false)
                {
                    throw new InvalidOperationException("Users must have the same schedule");
                }
                group.Users = context.Users.Where(w => request.Users.Contains(w.Id)).Distinct().ToList();

                if (group.ScheduleId != null)
                {
                    PopulateScheduleId(group.Users, group.ScheduleId);
                }
            }
        }

        if (group.GroupType == GroupType.GroupOfGroups)
        {



            if (request.Groups != null)
            {


                if (CheckIfAllGroupsHaveSameSchedule(group.ScheduleId, request.Groups) == false)
                {
                    throw new InvalidOperationException("Groups must have the same schedule");
                }

                group.Groups = context.Groups.Where(w => request.Groups.Contains(w.Id)).ToList();

                if (group.ScheduleId != null)
                {

                    PopulateScheduleId(group.Groups, group.ScheduleId);
                }
            }
        }

        context.SaveChanges();

        return group;
    }
}