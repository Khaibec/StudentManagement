using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Api.DTOs.Common;
using StudentManagement.Api.DTOs.Dashboard;
using StudentManagement.Api.Services.Interfaces;

namespace StudentManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// Lấy số liệu thống kê tổng quan (Học sinh, Lớp học, Môn học, Đăng ký)
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse<DashboardStatsDto>>> GetStats()
    {
        var stats = await _dashboardService.GetDashboardStatsAsync();
        return Ok(ApiResponse<DashboardStatsDto>.Ok(stats, "Lấy dữ liệu thống kê thành công."));
    }
}
