using AMS.DTO;
using AMS.Models;
using AMS.Requests;
using AMS.Responses;
using AutoMapper;

namespace AMS.Utils
{
    public class Mapper : Profile
    {

        public Mapper()
        {
            // transform role ids to roles
            CreateMap<AddUserRequest, ApplicationUser>();

            CreateMap<LoginDTO, LoginResponse>();
            CreateMap<ApplicationUser, UserResponse>();
            CreateMap<ApplicationUser, UserSummaryResponse>();
            CreateMap<ApplicationUser, UserBaseSummaryResponse>();
            CreateMap<Role, RoleResponse>();
            CreateMap<Role, RoleSummaryResponse>();
            CreateMap<Role, RoleDetailResponse>();
            CreateMap<Permission, PermissionResponse>();
            CreateMap<Schedule, ScheduleResponse>();
            CreateMap<Group, GroupResponse>();
            CreateMap<Group, GroupSummaryResponse>();

            // create map for paginationDTO
            CreateMap<PaginationDTO<ApplicationUser>, PaginationDTO<UserSummaryResponse>>();
            CreateMap<PaginationDTO<Group>, PaginationDTO<GroupSummaryResponse>>();

            CreateMap<Schedule, ScheduleResponse>();
            CreateMap<Schedule, ScheduleDetailResponse>();
            CreateMap<Session, SessionResponse>();
            CreateMap<Session, SessionDetailResponse>();
            CreateMap<Slot, SlotResponse>();
            CreateMap<TimeSlot, TimeSlotResponse>();
            CreateMap<Topic, TopicResponse>();

        }
    }
}
