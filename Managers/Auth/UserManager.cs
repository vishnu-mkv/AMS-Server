using AMS.Data;
using AMS.DTO;
using AMS.Interfaces;
using AMS.Migrations;
using AMS.Models;
using AMS.Requests;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;

namespace AMS.Managers.Auth
{
    public class UserManager : IUserManager
    {

        private readonly ISecurePasswordHasher passwordHasher;
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IRoleProvider rolePermissionProvider;
        private readonly IFileManager fileManager;
        private readonly IScheduleManager scheduleManager;
        private readonly IAuthManager authManager;

        public UserManager(ISecurePasswordHasher passwordHasher, ApplicationDbContext dbContext,
             IMapper mapper,
             IRoleProvider rolePermissionProvider, IFileManager fileManager,
             IScheduleManager scheduleManager, IAuthManager authManager)
        {
            this.passwordHasher = passwordHasher;
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.rolePermissionProvider = rolePermissionProvider;
            this.fileManager = fileManager;
            this.scheduleManager = scheduleManager;
            this.authManager = authManager;
        }

        public ApplicationUser? GetUserById(string id, bool includeRoles = false, bool includeSchedule = false, bool includeGroups = false)
        {
            var query = dbContext.Users.AsQueryable();

            if (includeRoles) query = query.Include(u => u.Roles);

            if (includeSchedule) query = query.Include(u => u.Schedule);

            if (includeGroups) query = query.Include(u => u.Groups);

            return query.FirstOrDefault(u => u.Id == id);
        }

        public ApplicationUser? GetUserByUsername(string username)
        {
            return dbContext.Users.FirstOrDefault(u => u.UserName == username);
        }

        public bool IsUniqueID(string id)
        {
            return dbContext.Users.Find(id) == null;
        }

        public bool IsUniqueUserName(string username)
        {
            return dbContext.Users.FirstOrDefault(u => u.UserName == username) == null;
        }



        private Schedule CheckSchedule(string scheduleId, string organizationId)
        {

            Schedule schedule = scheduleManager.GetSchedule(scheduleId) ?? throw new Exception("Schedule not found.");

            if (schedule.OrganizationId != organizationId) throw new Exception("Schedule does not belong to the organization.");

            return schedule;
        }

        private void CheckGroupScheduleId(ApplicationUser user, string[] GroupIds)
        {
            var groups = dbContext.Groups.Where(g => GroupIds.Contains(g.Id)).Include(g => g.Schedule).Distinct().ToList();

            // select shcedules from the groups
            var schedules = groups.Select(g => g.ScheduleId).Distinct().ToList();

            // check if the user has a schedule
            if (schedules.Count > 1) throw new Exception("User can not be in groups with different schedules.");


            if (user.ScheduleId != null && schedules[0] != user.ScheduleId) throw new Exception("User schedule does not match the group schedules.");

            if (user.Schedule == null) user.ScheduleId = schedules[0];

            user.Groups = groups;
        }

        public ApplicationUser Register(AddUserRequest request, Organization organization)
        {

            if (request.Id != null && !IsUniqueID(request.Id)) throw new Exception("User ID is not unique.");

            var user = mapper.Map<ApplicationUser>(request) ?? throw new Exception(
                "User is null. Check if the request is valid and if the user is not already registered.");

            user.OrganizationId = organization.Id;

            if (request.RoleIds != null)
                user.Roles = dbContext.Roles.Where(r => request.RoleIds.Contains(r.Id)).ToList();

            if (request.Password != null)
            {
                user.Password = passwordHasher.Hash(request.Password);
            }

            if (request.PictureFile != null)
            {
                user.Picture = fileManager.Save("profile", request.PictureFile);
            }

            if (request.ScheduleId != null)
            {
                user.Schedule = CheckSchedule(request.ScheduleId, authManager.GetUserOrganizationId());
            }

            if (request.GroupIds != null)
            {

                CheckGroupScheduleId(user, request.GroupIds);
            }

            dbContext.Users.Add(user);
            dbContext.SaveChanges();

            user.Organization = organization;

            // populate the roles and permissions

            user.Roles = rolePermissionProvider.GetRolesByIds(request.RoleIds ?? Array.Empty<string>());
            return user;
        }

        public ApplicationUser Register(AddUserRequest request)
        {
            return Register(request, authManager.GetUserOrganization());
        }



        public ApplicationUser UpdateUser(UpdateUserRequest request, string id)
        {
            var user = dbContext.Users.Include(u => u.Roles).Include(u => u.Groups).FirstOrDefault(u => u.Id == id) ?? throw new Exception("User not found.");

            if (request.FirstName != null) user.FirstName = request.FirstName;

            if (request.LastName != null) user.LastName = request.LastName;

            if (request.Password != null) user.Password = passwordHasher.Hash(request.Password);

            // DOB
            if (request.DOB != null) user.DOB = request.DOB;

            //signin
            if (request.SignInAllowed != null) user.SignInAllowed = (bool)request.SignInAllowed;

            if (request.UserName != null &&
                request.UserName != user.UserName &&
                !IsUniqueUserName(request.UserName)) user.UserName = request.UserName;

            if (request.RoleIds != null)
            {
                user.Roles = dbContext.Roles.Where(r => request.RoleIds.Contains(r.Id)).ToList();
            }

            if (request.PictureFile != null)
            {
                var oldFile = user.Picture;
                user.Picture = fileManager.Save("profile", request.PictureFile);
                if (oldFile != null) fileManager.DeleteFile(oldFile);
            }

            // gender and disabled
            if (request.Gender != null) user.Gender = (Enums.Gender)request.Gender;

            if (request.Disabled != null) user.Disabled = (bool)request.Disabled;

            if (request.ScheduleId != null) user.Schedule = CheckSchedule(request.ScheduleId, authManager.GetUserOrganizationId());

            if (request.GroupIds != null)
            {
                CheckGroupScheduleId(user, request.GroupIds);
            }

            dbContext.SaveChanges();

            user.Roles = rolePermissionProvider.GetRolesByIds(user.Roles.Select(r => r.Id).ToArray());

            user.Organization = authManager.GetUserOrganization();

            return user;
        }

        public PaginationDTO<ApplicationUser> ListUsers(UserPaginationQuery paginationQuery)
        {
            var organization = authManager.GetUserOrganization();

            IQueryable<ApplicationUser> query = dbContext.Users.Include(u => u.Organization).Include(u => u.Roles);

            // Filter by ID
            query = query.Where(user => user.OrganizationId == organization.Id);

            // Apply roles filtering
            if (paginationQuery.Roles.Length > 0)
            {
                query = query.Where(user => user.Roles.Any(role => paginationQuery.Roles.Contains(role.Id)));
            }


            // Apply search filtering
            if (!string.IsNullOrEmpty(paginationQuery.Search))
            {
                // search string in title case
                TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                string search = myTI.ToTitleCase(paginationQuery.Search);
                query = query.Where(user =>
                    user.FirstName.Contains(search.ToLower()) ||
                    user.FirstName.Contains(search) ||
                    user.LastName.Contains(search) ||
                    user.LastName.Contains(search.ToLower())
                    );
            }

            var paginationOrder = paginationQuery.Order.ToLower();

            // Apply sorting
            switch (paginationQuery.SortBy)
            {
                case "createdAt":
                    if (paginationOrder == "desc")
                        query = query.OrderByDescending(user => user.CreatedAt);
                    else
                        query = query.OrderBy(user => user.CreatedAt);
                    break;
                case "name":
                    if (paginationOrder == "desc")
                        query = query.OrderByDescending(user => user.FirstName).ThenByDescending(user => user.LastName);
                    else
                        query = query.OrderBy(user => user.FirstName).ThenBy(user => user.LastName);
                    break;
                // Add more cases for additional sorting fields if needed
                default:
                    query = query.OrderBy(user => user.CreatedAt);
                    break;
            }

            // Execute the final query and retrieve the paginated results
            var paginationDTO = new PaginationDTO<ApplicationUser>(query, paginationQuery);

            return paginationDTO;
        }

        public bool UserExists(string organizationId, string id)
        {
            return dbContext.Users.Any(u => u.Id == id && u.OrganizationId == organizationId);
        }

        public bool CheckUsersHaveSchedule(string? scheduleId, string[] users)
        {
            return dbContext.Users.Where(u => users.Contains(u.Id)).All(u => u.ScheduleId == scheduleId);
        }
    }
}
