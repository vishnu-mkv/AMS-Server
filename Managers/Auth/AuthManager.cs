using System.Security.Claims;
using AMS.Data;
using AMS.DTO;
using AMS.Interfaces;
using AMS.Models;
using Microsoft.EntityFrameworkCore;

namespace AMS.Managers.Auth;

public class AuthManager : IAuthManager
{
    private readonly ISecurePasswordHasher passwordHasher;
    private readonly ApplicationDbContext dbContext;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IJwtProvider jwtProvider;
    private readonly IOrganizationProvider organizationProvider;


    public AuthManager(ApplicationDbContext dbContext, ISecurePasswordHasher passwordHasher, IHttpContextAccessor httpContextAccessor, IJwtProvider jwtProvider, IOrganizationProvider organizationProvider)
    {
        this.dbContext = dbContext;
        this.passwordHasher = passwordHasher;
        this.httpContextAccessor = httpContextAccessor;
        this.jwtProvider = jwtProvider;
        this.organizationProvider = organizationProvider;
    }

    public ApplicationUser? GetCurrentUser()
    {
        string userId = GetCurrentUserId();
        return dbContext.Users.Find(userId);
    }
    public string GetCurrentUserId()
    {
        var id = httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

        if (id == null) throw new Exception("User is not authenticated.");

        return id;
    }


    public string GetUserOrganizationId()
    {
        var organizationId = httpContextAccessor?.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.PrimaryGroupSid)?.Value;

        if (organizationId == null) throw new Exception("User is not in an organization.");
        return organizationId;
    }

    public Organization GetUserOrganization()
    {
        var organizationId = GetUserOrganizationId();
        return organizationProvider.GetOrganizationById(organizationId) ?? throw new Exception("Organization not found.");
    }

    public LoginDTO Login(string username, string password)
    {
        ApplicationUser user = dbContext.Users.Include(u => u.Organization).Include(u => u.Roles).ThenInclude(r => r.Permissions).FirstOrDefault(u => u.UserName == username) ?? throw new Exception("User not found.");

        if (!passwordHasher.Verify(password, user.Password))
            throw new Exception("Invalid password.");

        var token = jwtProvider.GenerateToken(user);

        return new LoginDTO
        {
            Token = token,
            User = user
        };

    }
}