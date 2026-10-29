using Microsoft.EntityFrameworkCore;
using StudentManagement.Api.Entities;

namespace StudentManagement.Api.Data;

// DbContext đóng vai trò là "cầu nối" giữa C# code và Database SQL Server.
// Nó theo dõi sự thay đổi của dữ liệu và sinh ra các câu lệnh SQL tương ứng.
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSet<T> đại diện cho một bảng trong CSDL. Thông qua DbSet, ta có thể Query, Thêm, Sửa, Xóa dữ liệu.
    public DbSet<User> Users => Set<User>();
    public DbSet<ClassRoom> Classes => Set<ClassRoom>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<UserExternalAccount> UserExternalAccounts => Set<UserExternalAccount>();

    // OnModelCreating sử dụng "Fluent API" để tùy biến cấu trúc database (ràng buộc, khóa ngoại, quan hệ...)
    // Cách này linh hoạt và chuyên nghiệp hơn so với việc gắn Data Annotations trực tiếp vào model.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Cấu hình bảng User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique(); // Đảm bảo tên đăng nhập không bị trùng lặp
            entity.Property(u => u.Username).HasMaxLength(50).IsRequired();
            entity.Property(u => u.FullName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.Role).HasMaxLength(20).IsRequired();
        });

        // 2. Cấu hình bảng ClassRoom
        modelBuilder.Entity<ClassRoom>(entity =>
        {
            entity.Property(c => c.Name).HasMaxLength(50).IsRequired();
            entity.Property(c => c.Description).HasMaxLength(200);
        });

        // 3. Cấu hình bảng Student
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasIndex(s => s.StudentCode).IsUnique(); // Mã học sinh phải là duy nhất
            entity.Property(s => s.StudentCode).HasMaxLength(20).IsRequired();
            entity.Property(s => s.FullName).HasMaxLength(100).IsRequired();
            entity.Property(s => s.Gender).HasMaxLength(10).IsRequired();
            entity.Property(s => s.Email).HasMaxLength(100);
            entity.Property(s => s.PhoneNumber).HasMaxLength(20);
            entity.Property(s => s.Address).HasMaxLength(250);

            // Quan hệ 1-N: Một ClassRoom có nhiều Students, mỗi Student thuộc về một ClassRoom
            // DeleteBehavior.SetNull: Khi xóa một Lớp học, học sinh thuộc lớp đó KHÔNG bị xóa 
            // mà trường ClassRoomId của học sinh sẽ tự động được gán về null.
            entity.HasOne(s => s.ClassRoom)
                  .WithMany(c => c.Students)
                  .HasForeignKey(s => s.ClassRoomId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // 4. Cấu hình bảng Course
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasIndex(c => c.Code).IsUnique(); // Mã môn học duy nhất
            entity.Property(c => c.Code).HasMaxLength(20).IsRequired();
            entity.Property(c => c.Title).HasMaxLength(150).IsRequired();
            entity.Property(c => c.Description).HasMaxLength(500);
        });

        // 5. Cấu hình bảng Enrollment (bảng trung gian thể hiện quan hệ N-N giữa Student và Course)
        modelBuilder.Entity<Enrollment>(entity =>
        {
            // Ràng buộc Unique kết hợp (Composite Index): Một học sinh chỉ được đăng ký môn học đó 1 lần
            entity.HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();

            // Quan hệ với Student:
            // DeleteBehavior.Cascade: Khi một học sinh bị xóa, tất cả bản ghi đăng ký khóa học của học sinh đó cũng tự động bị xóa theo.
            entity.HasOne(e => e.Student)
                  .WithMany(s => s.Enrollments)
                  .HasForeignKey(e => e.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ với Course:
            // DeleteBehavior.Cascade: Khi một môn học bị xóa, tất cả bản ghi đăng ký của môn học đó cũng tự động bị xóa theo.
            entity.HasOne(e => e.Course)
                  .WithMany(c => c.Enrollments)
                  .HasForeignKey(e => e.CourseId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 6. Cấu hình bảng UserExternalAccount (Liên kết tài khoản mạng xã hội OAuth)
        modelBuilder.Entity<UserExternalAccount>(entity =>
        {
            // Đảm bảo 1 tài khoản MXH không bị trùng lặp trong hệ thống
            entity.HasIndex(e => new { e.Provider, e.ProviderKey }).IsUnique();
            entity.Property(e => e.Provider).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ProviderKey).HasMaxLength(150).IsRequired();
            entity.Property(e => e.ProviderDisplayName).HasMaxLength(150);
            entity.Property(e => e.ProviderEmail).HasMaxLength(150);

            entity.HasOne(e => e.User)
                  .WithMany(u => u.ExternalAccounts)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
