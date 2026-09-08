using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Api.DTOs.Common;
using StudentManagement.Api.DTOs.Enrollments;
using StudentManagement.Api.Services.Interfaces;

namespace StudentManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    /// <summary>
    /// Lấy danh sách đăng ký môn học (có thể lọc theo studentId hoặc courseId)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<EnrollmentDto>>>> GetAll(
        [FromQuery] int? studentId,
        [FromQuery] int? courseId)
    {
        var items = await _enrollmentService.GetEnrollmentsAsync(studentId, courseId);
        return Ok(ApiResponse<IEnumerable<EnrollmentDto>>.Ok(items, "Lấy danh sách đăng ký thành công."));
    }

    /// <summary>
    /// Đăng ký học sinh vào khóa học/môn học (Chỉ Admin)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<EnrollmentDto>>> Enroll([FromBody] CreateEnrollmentDto dto)
    {
        var created = await _enrollmentService.EnrollStudentAsync(dto);
        return Ok(ApiResponse<EnrollmentDto>.Ok(created, "Đăng ký môn học thành công!"));
    }

    /// <summary>
    /// Cập nhật/Nhập điểm cho học sinh (Chỉ Admin)
    /// </summary>
    [HttpPut("{id}/grade")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<EnrollmentDto>>> UpdateGrade(int id, [FromBody] UpdateGradeDto dto)
    {
        var updated = await _enrollmentService.UpdateGradeAsync(id, dto);
        return Ok(ApiResponse<EnrollmentDto>.Ok(updated, "Cập nhật điểm thành công!"));
    }

    /// <summary>
    /// Hủy đăng ký môn học (Chỉ Admin)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> Cancel(int id)
    {
        await _enrollmentService.CancelEnrollmentAsync(id);
        return Ok(ApiResponse<bool>.Ok(true, "Hủy đăng ký môn học thành công!"));
    }
}
