using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Api.DTOs.Classes;
using StudentManagement.Api.DTOs.Common;
using StudentManagement.Api.Services.Interfaces;

namespace StudentManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Yêu cầu người dùng phải đăng nhập để xem danh sách
public class ClassesController : ControllerBase
{
    private readonly IClassService _classService;

    public ClassesController(IClassService classService)
    {
        _classService = classService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ClassDto>>>> GetAll()
    {
        var classes = await _classService.GetAllClassesAsync();
        return Ok(ApiResponse<IEnumerable<ClassDto>>.Ok(classes, "Lấy danh sách lớp học thành công."));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ClassDto>>> GetById(int id)
    {
        var item = await _classService.GetClassByIdAsync(id);
        return Ok(ApiResponse<ClassDto>.Ok(item, "Lấy chi tiết lớp học thành công."));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")] // Chỉ Admin mới có quyền thêm lớp học
    public async Task<ActionResult<ApiResponse<ClassDto>>> Create([FromBody] CreateClassDto dto)
    {
        var created = await _classService.CreateClassAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<ClassDto>.Ok(created, "Thêm lớp học thành công!"));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")] // Chỉ Admin mới có quyền sửa lớp học
    public async Task<ActionResult<ApiResponse<ClassDto>>> Update(int id, [FromBody] UpdateClassDto dto)
    {
        var updated = await _classService.UpdateClassAsync(id, dto);
        return Ok(ApiResponse<ClassDto>.Ok(updated, "Cập nhật lớp học thành công!"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")] // Chỉ Admin mới có quyền xóa lớp học
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        await _classService.DeleteClassAsync(id);
        return Ok(ApiResponse<bool>.Ok(true, "Xóa lớp học thành công!"));
    }
}
