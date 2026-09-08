using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Api.DTOs.Common;
using StudentManagement.Api.DTOs.Courses;
using StudentManagement.Api.Services.Interfaces;

namespace StudentManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<CourseDto>>>> GetAll()
    {
        var courses = await _courseService.GetAllCoursesAsync();
        return Ok(ApiResponse<IEnumerable<CourseDto>>.Ok(courses, "Lấy danh sách môn học thành công."));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CourseDto>>> GetById(int id)
    {
        var item = await _courseService.GetCourseByIdAsync(id);
        return Ok(ApiResponse<CourseDto>.Ok(item, "Lấy chi tiết môn học thành công."));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<CourseDto>>> Create([FromBody] CreateCourseDto dto)
    {
        var created = await _courseService.CreateCourseAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<CourseDto>.Ok(created, "Thêm môn học thành công!"));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<CourseDto>>> Update(int id, [FromBody] UpdateCourseDto dto)
    {
        var updated = await _courseService.UpdateCourseAsync(id, dto);
        return Ok(ApiResponse<CourseDto>.Ok(updated, "Cập nhật môn học thành công!"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        await _courseService.DeleteCourseAsync(id);
        return Ok(ApiResponse<bool>.Ok(true, "Xóa môn học thành công!"));
    }
}
