using System.Text.Json.Serialization;
using AMS.Utils;

namespace AMS.Responses;

public class UserBaseSummaryResponse
{

    // id, first name, last name, username, roles, profile picture
    public string Id { get; set; } = null!;

    public string FirstName { get; set; }

    public string LastName { get; set; }

    [JsonConverter(typeof(PhotoUrlConverter))]
    public string? Picture { get; set; }

    public bool disabled { get; set; }
}

public class UserSummaryResponse : UserBaseSummaryResponse
{
    public RoleSummaryResponse[] Roles { get; set; } = Array.Empty<RoleSummaryResponse>();

}