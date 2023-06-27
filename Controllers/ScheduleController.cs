namespace AMS.Controllers;

using AMS.Authorization;
using AMS.Interfaces;
using AMS.Providers;
using AMS.Requests;
using AMS.Responses;
using AMS.Validators;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

// controller for schedule

[ApiController]
[Route("/api/schedules")]
public class ScheduleController : ControllerBase
{

    private readonly IScheduleManager scheduleManager;
    private readonly AddScheduleValidator addScheduleValidator;
    private readonly IMapper mapper;
    public ScheduleController(IScheduleManager scheduleManager, AddScheduleValidator addScheduleValidator, IMapper mapper)
    {
        this.scheduleManager = scheduleManager;
        this.addScheduleValidator = addScheduleValidator;
        this.mapper = mapper;
    }

    [HttpPost]
    [HasPermission(PermissionEnum.AddSchedule)]
    public IActionResult AddSchedule([FromBody] AddScheduleRequest request)
    {
        // validate request
        var validationResult = addScheduleValidator.Validate(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var schedule = scheduleManager.AddSchedule(request);

        // save to database
        return Ok(mapper.Map<ScheduleResponse>(schedule));
    }

    [HttpGet]
    [Route("")]
    [HasPermission(PermissionEnum.ListSchedules)]
    public IActionResult ListSchedules()
    {
        return Ok(mapper.Map<List<ScheduleResponse>>(scheduleManager.GetSchedules()));
    }
}