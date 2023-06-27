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

            // create map for paginationDTO
            CreateMap<PaginationDTO<ApplicationUser>, PaginationDTO<UserSummaryResponse>>();
        }
    }
}
