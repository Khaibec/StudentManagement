using StudentManagement.Api.DTOs.Dashboard;

namespace StudentManagement.Api.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
}
