using AMS.Authorization;
using AMS.DTO;
using AMS.Interfaces;
using AMS.Models;
using AMS.Providers;
using AMS.Requests;
using AMS.Responses;
using AMS.Validators;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Controllers
{
    [ApiController]
    [Route("/api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserValidator _userValidator;
        private readonly IUserManager _userManager;
        private readonly IMapper _mapper;
        private readonly UpdateUserValidator _updateUserValidator;
        private readonly IAuthManager _authManager;

        public UsersController(UserValidator userValidator, IUserManager userManager, IMapper mapper, UpdateUserValidator updateUserValidator, IAuthManager authManager)
        {
            _userValidator = userValidator;
            _userManager = userManager;
            _mapper = mapper;
            _updateUserValidator = updateUserValidator;
            _authManager = authManager;
        }

        // POST api/<UsersController>/
        [HttpPost]
        [Route("")]
        [HasPermission(PermissionEnum.AddUser)]
        public ActionResult<ApplicationUser> AddUser([FromForm] AddUserRequest request)
        {

            if (request.Id == null)
                request.Id = Guid.NewGuid().ToString();


            var result = _userValidator.Validate(request);

            if (!result.IsValid)
            {
                throw new Exception(result.Errors.First().ErrorMessage);
            }


            var user = _userManager.Register(request);

            return Ok(_mapper.Map<UserResponse>(user));
        }

        // list users
        // GET api/<UsersController>/
        [HttpGet]
        [Route("")]
        [HasPermission(PermissionEnum.ListUsers)]
        public ActionResult<List<UserSummaryResponse>> ListUsers(
            [FromQuery] UserPaginationQuery paginationQuery)

        {
            // pagination query
            var users = _userManager.ListUsers(paginationQuery);

            return Ok(_mapper.Map<PaginationDTO<UserSummaryResponse>>(users));
        }


        // PATCH api/<UsersController>/:id
        [HttpPatch]
        [Route("{id}")]
        [HasPermission(PermissionEnum.UpdateUser)]
        public ActionResult<UserResponse> UpdateUser([FromForm] UpdateUserRequest request, [FromRoute] string id)
        {
            var result = _updateUserValidator.Validate(request);

            if (!result.IsValid)
            {
                throw new Exception(result.Errors.First().ErrorMessage);
            }

            var user = _userManager.UpdateUser(request, id);

            return Ok(_mapper.Map<UserResponse>(user));
        }

        // get user by id
        // GET api/<UsersController>/:id
        [HttpGet]
        [Route("{id}")]
        public ActionResult<ApplicationUser> GetUser([FromRoute] string id)
        {
            var user = _userManager.GetUserById(id, includeRoles: true, includeGroups: true, includeSchedule: true);

            return Ok(_mapper.Map<UserResponse>(user));
        }


        // POST api/<UsersController>/login
        [HttpPost]
        [Route("login")]
        public ActionResult<LoginResponse> LoginUser([FromBody] LoginRequest request)
        {
            var responseData = _authManager.Login(request.username, request.password);

            return Ok(_mapper.Map<LoginResponse>(responseData));
        }

    }
}
