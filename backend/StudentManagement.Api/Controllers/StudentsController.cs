using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Api.DTOs.Common;
using StudentManagement.Api.DTOs.Students;
using StudentManagement.Api.Services.Interfaces;

namespace StudentManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    /// <summary>
    /// Lấy danh sách học sinh có hỗ trợ: Tìm kiếm, Lọc theo lớp/giới tính, Sắp xếp và Phân trang
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<StudentDto>>>> GetAll([FromQuery] StudentQueryParameters query)
    {
        var result = await _studentService.GetStudentsAsync(query);
        return Ok(ApiResponse<PagedResult<StudentDto>>.Ok(result, "Lấy danh sách học sinh thành công."));
    }

    /// <summary>
    /// Lấy thông tin chi tiết học sinh kèm danh sách các môn đã đăng ký và điểm số
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<StudentDetailDto>>> GetById(int id)
    {
        var item = await _studentService.GetStudentByIdAsync(id);
        return Ok(ApiResponse<StudentDetailDto>.Ok(item, "Lấy chi tiết học sinh thành công."));
    }

    /// <summary>
    /// Thêm học sinh mới (Chỉ Admin)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<StudentDto>>> Create([FromBody] CreateStudentDto dto)
    {
        var created = await _studentService.CreateStudentAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<StudentDto>.Ok(created, "Thêm học sinh thành công!"));
    }

    /// <summary>
    /// Cập nhật thông tin học sinh (Chỉ Admin)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<StudentDto>>> Update(int id, [FromBody] UpdateStudentDto dto)
    {
        var updated = await _studentService.UpdateStudentAsync(id, dto);
        return Ok(ApiResponse<StudentDto>.Ok(updated, "Cập nhật học sinh thành công!"));
    }

    /// <summary>
    /// Xóa học sinh (Chỉ Admin)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        await _studentService.DeleteStudentAsync(id);
        return Ok(ApiResponse<bool>.Ok(true, "Xóa học sinh thành công!"));
    }
}
