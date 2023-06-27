using AMS.Data;
using AMS.Interfaces;
using AMS.Models;
using Microsoft.EntityFrameworkCore;

namespace AMS.Providers;

public class OrganizationProvider : IOrganizationProvider
{

    private readonly Dictionary<string, Organization> _organizationMap;
    private readonly IServiceScopeFactory _scopeFactory;

    public OrganizationProvider(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        using IServiceScope scope = scopeFactory.CreateScope();

        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        _organizationMap = dbContext.Organizations.AsNoTracking().ToDictionary(o => o.Id.ToString());
    }

    public Organization GetOrganizationById(string id)
    {
        return _organizationMap.GetValueOrDefault(id);
    }

    // create a new organization
    // input: organization name
    // output: organization
    public Organization CreateOrganization(string name)
    {
        Organization organization = new(name);

        using IServiceScope scope = _scopeFactory.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Organizations.Add(organization);
        dbContext.SaveChanges();

        _organizationMap.Add(organization.Id.ToString(), organization);
        return organization;
    }

}