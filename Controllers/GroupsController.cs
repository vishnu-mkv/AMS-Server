using System.Text.RegularExpressions;
using AMS.Authorization;
using AMS.DTO;
using AMS.Interfaces;
using AMS.Providers;
using AMS.Requests;
using AMS.Responses;
using AMS.Validators;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{

    private readonly IGroupManager _groupManager;
    private readonly AddGroupValidaor addGroupValidaor;
    private readonly UpdateGroupValidaor updateGroupValidaor;
    private readonly IMapper mapper;

    public GroupsController(IGroupManager groupManager, AddGroupValidaor addGroupValidaor, UpdateGroupValidaor updateGroupValidaor, IMapper mapper)
    {
        _groupManager = groupManager;
        this.addGroupValidaor = addGroupValidaor;
        this.updateGroupValidaor = updateGroupValidaor;
        this.mapper = mapper;
    }

    [HttpGet("{id}")]
    [HasPermission(PermissionEnum.ReadGroup)]
    public IActionResult GetGroup(string id)
    {
        return Ok(mapper.Map<GroupResponse>(_groupManager.GetGroup(id)));
    }

    [HttpPost]
    [HasPermission(PermissionEnum.AddGroup)]
    public ActionResult<Group> CreateGroup(AddGroupRequest request)
    {
        var result = addGroupValidaor.Validate(request);

        if (!result.IsValid)
        {
            return BadRequest(result.Errors);
        }

        return Ok(mapper.Map<GroupResponse>(_groupManager.CreateGroup(request)));
    }

    [HttpPut("{id}")]
    [HasPermission(PermissionEnum.UpdateGroup)]
    public ActionResult<Group> UpdateGroup(UpdateGroupRequest request, string id)
    {
        var result = updateGroupValidaor.Validate(request);

        if (!result.IsValid)
        {
            return BadRequest(result.Errors);
        }

        return Ok(mapper.Map<GroupResponse>(_groupManager.UpdateGroup(request, id)));
    }

    // list of groups
    // with pagination
    [HttpGet]
    [HasPermission(PermissionEnum.ListGroups)]
    public IActionResult ListGroups([FromQuery] GroupPaginationQuery paginationQuery)
    {
        return Ok(mapper.Map<PaginationDTO<GroupSummaryResponse>>(_groupManager.GetGroups(paginationQuery)));
    }
}