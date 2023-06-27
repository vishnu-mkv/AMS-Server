using AMS.Models;

namespace AMS.Interfaces;

public interface IOrganizationProvider
{
    Organization GetOrganizationById(string id);

    public Organization CreateOrganization(string name);

}