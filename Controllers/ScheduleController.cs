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
    private readonly UpdateScheduleValidator updateScheduleValidator;
    private readonly AddTimeSlotValidator addTimeSlotValidator;
    private readonly AddSessionValidator addSessionValidator;
    private readonly IMapper mapper;
    private readonly ITimeSlotManager timeSlotManager;
    private readonly ISessionManager sessionManager;

    public ScheduleController(IScheduleManager scheduleManager, AddScheduleValidator addScheduleValidator, IMapper mapper, UpdateScheduleValidator updateScheduleValidator, AddTimeSlotValidator addTimeSlotValidator, AddSessionValidator addSessionValidator, ISessionManager sessionManager, ITimeSlotManager timeSlotManager)
    {
        this.scheduleManager = scheduleManager;
        this.addScheduleValidator = addScheduleValidator;
        this.mapper = mapper;
        this.updateScheduleValidator = updateScheduleValidator;
        this.addTimeSlotValidator = addTimeSlotValidator;
        this.addSessionValidator = addSessionValidator;
        this.sessionManager = sessionManager;
        this.timeSlotManager = timeSlotManager;
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

    [HttpPut]
    [Route("{id}")]
    [HasPermission(PermissionEnum.UpdateSchedule)]
    public IActionResult UpdateSchedule([FromBody] UpdateScheduleRequest request, [FromRoute] string id)
    {

        var validationResult = updateScheduleValidator.Validate(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var schedule = scheduleManager.UpdateSchedule(id, request);
        return Ok(mapper.Map<ScheduleResponse>(schedule));

    }

    [HttpGet]
    [Route("{id}")]
    public IActionResult GetSchedule([FromRoute] string id)
    {
        var schedule = scheduleManager.GetSchedule(id, true);
        return Ok(mapper.Map<ScheduleDetailResponse>(schedule));
    }


    [HttpGet]
    [Route("")]
    [HasPermission(PermissionEnum.ListSchedules)]
    public IActionResult ListSchedules()
    {
        return Ok(mapper.Map<List<ScheduleResponse>>(scheduleManager.GetSchedules()));
    }

    // add, remove timeslot
    [HttpPost]
    [Route("{id}/timeslots")]
    [HasPermission(PermissionEnum.AddTimeSlot)]
    public IActionResult AddTimeSlot([FromBody] AddTimeSlotRequest request, [FromRoute] string id)
    {
        var validationResult = addTimeSlotValidator.Validate(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var schedule = timeSlotManager.AddTimeSlot(request, id);
        return Ok(mapper.Map<TimeSlotResponse>(schedule));
    }

    [HttpDelete]
    [Route("{id}/timeslots/{timeSlotId}")]
    [HasPermission(PermissionEnum.DeleteTimeSlot)]
    public IActionResult DeleteTimeSlot([FromRoute] string id, [FromRoute] string timeSlotId)
    {
        timeSlotManager.DeleteTimeSlot(timeSlotId);
        return Ok(true);
    }

    // get, create, update, delete session
    [HttpPost]
    [Route("sessions")]
    [HasPermission(PermissionEnum.AddSession)]
    public IActionResult AddSession([FromBody] AddSessionRequest request)
    {
        var validationResult = addSessionValidator.Validate(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var session = sessionManager.AddSession(request, request.ScheduleId);
        return Ok(mapper.Map<SessionResponse>(session));
    }

    [HttpPut]
    [Route("sessions/{sessionId}")]
    [HasPermission(PermissionEnum.UpdateSession)]
    public IActionResult UpdateSession([FromBody] AddSessionRequest request, [FromRoute] string sessionId)
    {
        var validationResult = addSessionValidator.Validate(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var session = sessionManager.UpdateSession(sessionId, request);
        return Ok(mapper.Map<SessionResponse>(session));
    }

    [HttpDelete]
    [Route("sessions/{sessionId}")]
    [HasPermission(PermissionEnum.DeleteSession)]
    public IActionResult DeleteSession([FromRoute] string sessionId)
    {
        sessionManager.DeleteSession(sessionId);
        return Ok(true);
    }

    [HttpGet]
    [Route("sessions/{sessionId}")]
    [HasPermission(PermissionEnum.ReadSession)]
    public IActionResult GetSession([FromRoute] string sessionId)
    {
        return Ok(mapper.Map<SessionDetailResponse>(sessionManager.GetSession(sessionId, true)));
    }
}