namespace StudentManagement.Api.Repositories.Interfaces;

/// <summary>
/// Unit of Work pattern: Quản lý giao dịch và đảm bảo tính nhất quán của dữ liệu khi thao tác qua nhiều repository.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IStudentRepository Students { get; }
    IClassRepository Classes { get; }
    Task<int> CompleteAsync();
    Task<int> SaveChangesAsync();
}
