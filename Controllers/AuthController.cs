using AMS.Authorization;
using AMS.Interfaces;
using AMS.Providers;
using AMS.Requests;
using AMS.Responses;
using AMS.Validators;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly IRoleProvider _roleProvider;
        private readonly IMapper mapper;
        private readonly IRoleManager roleManager;
        private readonly AddRoleValidator addRoleValidator;
        private readonly UpdateRoleValidator updateRoleValidator;
        private readonly IAuthManager authManager;

        public AuthController(IRoleProvider roleProvider, IMapper mapper,
                                IRoleManager roleManager, AddRoleValidator addRoleValidator, UpdateRoleValidator updateRoleValidator,
                                IAuthManager authManager)
        {
            _roleProvider = roleProvider;
            this.mapper = mapper;
            this.roleManager = roleManager;
            this.addRoleValidator = addRoleValidator;
            this.updateRoleValidator = updateRoleValidator;
            this.authManager = authManager;
        }

        // get profile
        [HttpGet("profile")]
        [Authorize]
        public IActionResult GetProfile()
        {
            var user = authManager.GetProfile();
            return Ok(mapper.Map<UserResponse>(user));
        }


        // roles of organization
        [HttpGet("roles")]
        [HasPermission(PermissionEnum.ListRoles)]
        public IActionResult GetRoles()
        {
            var roles = roleManager.GetRoles();
            return Ok(mapper.Map<List<RoleSummaryResponse>>(roles));
        }

        // add role
        [HttpPost("roles")]
        [HasPermission(PermissionEnum.AddRole)]
        public IActionResult AddRole([FromBody] AddRoleRequest roleRequest)
        {

            // validate request
            var validationResult = addRoleValidator.Validate(roleRequest);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var role = roleManager.AddRole(roleRequest);
            return Ok(mapper.Map<RoleResponse>(role));
        }

        // detail role
        [HttpGet("roles/{id}")]
        public IActionResult GetRoleById([FromRoute] string id)
        {
            var role = roleManager.GetRoleById(id);
            return Ok(mapper.Map<RoleDetailResponse>(role));
        }

        // delete role
        [HttpDelete("roles/{id}")]
        [HasPermission(PermissionEnum.DeleteRole)]
        public IActionResult DeleteRole([FromRoute] string id)
        {
            roleManager.DeleteRole(id);
            return Ok(new { Ok = true, message = "The role was deleted successfully." });
        }

        // update role
        [HttpPatch("roles/{id}")]
        public IActionResult UpdateRole([FromBody] UpdateRoleRequest request, [FromRoute] string id)
        {
            // validate request
            var validationResult = updateRoleValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var role = roleManager.UpdateRole(request, id);
            return Ok(mapper.Map<RoleResponse>(role));
        }

        // list permissions
        [HttpGet("permissions")]
        [HasPermission(PermissionEnum.ListPermissions)]
        public IActionResult GetPermissions()
        {
            var permissions = _roleProvider.Permissions.Values.ToList();
            return Ok(mapper.Map<List<PermissionResponse>>(permissions));
        }
    }
}