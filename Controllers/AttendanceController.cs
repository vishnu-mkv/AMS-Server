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
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceManager _attendanceManager;
    private readonly IMapper _mapper;
    private readonly AddAttendanceValidator _addAttendanceValidator;
    private readonly UpdateAttendanceValidator _updateAttendanceValidator;
    private readonly IAttendanceStatusProvider _attendanceStatusProvider;

    public AttendanceController(IAttendanceManager attendanceManager, IMapper mapper, AddAttendanceValidator addAttendanceValidator, UpdateAttendanceValidator updateAttendanceValidator, IAttendanceStatusProvider attendanceStatusProvider)
    {
        _attendanceManager = attendanceManager;
        _mapper = mapper;
        _addAttendanceValidator = addAttendanceValidator;
        _updateAttendanceValidator = updateAttendanceValidator;
        _attendanceStatusProvider = attendanceStatusProvider;
    }

    [HttpGet("{attendanceId}")]
    [HasPermission(PermissionEnum.GetAttendance)]
    public IActionResult GetAttendance([FromRoute] string attendanceId)
    {
        var attendance = _attendanceManager.GetAttendance(attendanceId, true);
        if (attendance == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<AttendanceResponse>(attendance));
    }

    [HttpGet]
    [HasPermission(PermissionEnum.ListAttendances)]
    public IActionResult GetAttendances(
        [FromQuery] AttendancePaginationQuery AttendanceQuery
    )
    {
        var attendances = _attendanceManager.ListAttendances(AttendanceQuery);
        return Ok(_mapper.Map<PaginationDTO<AttendanceResponseWithoutRecords>>(attendances));
    }

    [HttpPost]
    [HasPermission(PermissionEnum.AddAttendance)]
    public IActionResult AddAttendance([FromBody] AddAttendanceRequest request)
    {
        var validationResult = _addAttendanceValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var attendance = _attendanceManager.AddAttendance(request);
        return CreatedAtAction(nameof(GetAttendance), new { attendanceId = attendance.Id }, _mapper.Map<AttendanceSummaryResponse>(attendance));
    }

    [HttpPut("{attendanceId}")]
    [HasPermission(PermissionEnum.UpdateAttendance)]
    public IActionResult UpdateAttendance([FromBody] UpdateAttendanceRequest request)
    {
        var validationResult = _updateAttendanceValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var attendance = _attendanceManager.UpdateAttendance(request);
        return Ok(_mapper.Map<AttendanceSummaryResponse>(attendance));
    }

    [HttpDelete("{attendanceId}")]
    [HasPermission(PermissionEnum.DeleteAttendance)]
    public IActionResult DeleteAttendance([FromRoute] string attendanceId)
    {
        _attendanceManager.DeleteAttendance(attendanceId);
        return NoContent();
    }

    // get attendance status
    [HttpGet("attendance-status")]
    [HasPermission(PermissionEnum.ListAttendanceStatuses)]
    public IActionResult GetAttendanceStatuses()
    {
        var attendanceStatuses = _attendanceStatusProvider.GetAllStatus();
        return Ok(attendanceStatuses);
    }

}