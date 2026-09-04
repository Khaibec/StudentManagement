using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StudentApi.DTOs;
using StudentApi.Services;

namespace StudentApi.Controllers;

[ApiController]
[Authorize]
[Route("api/enrollments")]
public class EnrollmentsController : ControllerBase
{
    private readonly EnrollmentService _service;

    public EnrollmentsController(EnrollmentService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<EnrollmentDto>>> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{studentId:int}/{courseId:int}")]
    public async Task<ActionResult<EnrollmentDto>> GetById(int studentId, int courseId)
    {
        var item = await _service.GetByIdAsync(studentId, courseId);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<EnrollmentDto>> Create(CreateEnrollmentDto dto)
    {
        var item = await _service.CreateAsync(dto);
        return item == null
            ? Conflict("Enrollment đã tồn tại.")
            : CreatedAtAction(nameof(GetById), new { dto.StudentId, dto.CourseId }, item);
    }

    [HttpPut("{studentId:int}/{courseId:int}")]
    public async Task<IActionResult> Update(int studentId, int courseId, UpdateEnrollmentDto dto)
        => await _service.UpdateAsync(studentId, courseId, dto) ? NoContent() : NotFound();

    [HttpDelete("{studentId:int}/{courseId:int}")]
    public async Task<IActionResult> Delete(int studentId, int courseId)
        => await _service.DeleteAsync(studentId, courseId) ? NoContent() : NotFound();
}
