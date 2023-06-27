using AMS.Models;

namespace AMS.Responses
{
    public class RoleSummaryResponse
    {

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Color { get; set; }

    }

    public class RoleResponse : RoleSummaryResponse
    {

        // permission response
        public PermissionResponse[] Permissions { get; set; } = Array.Empty<PermissionResponse>();
    }

    public class RoleDetailResponse : RoleResponse
    {

        public UserBaseSummaryResponse[] Users { get; set; } = Array.Empty<UserBaseSummaryResponse>();
    }
}
