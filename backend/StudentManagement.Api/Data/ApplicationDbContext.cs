using Microsoft.EntityFrameworkCore;
using StudentManagement.Api.Entities;

namespace StudentManagement.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<ClassRoom> Classes => Set<ClassRoom>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Cấu hình bảng User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
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
            entity.HasIndex(s => s.StudentCode).IsUnique();
            entity.Property(s => s.StudentCode).HasMaxLength(20).IsRequired();
            entity.Property(s => s.FullName).HasMaxLength(100).IsRequired();
            entity.Property(s => s.Gender).HasMaxLength(10).IsRequired();
            entity.Property(s => s.Email).HasMaxLength(100);
            entity.Property(s => s.PhoneNumber).HasMaxLength(20);
            entity.Property(s => s.Address).HasMaxLength(250);

            // Quan hệ 1-N: ClassRoom có nhiều Students
            entity.HasOne(s => s.ClassRoom)
                  .WithMany(c => c.Students)
                  .HasForeignKey(s => s.ClassRoomId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // 4. Cấu hình bảng Course
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasIndex(c => c.Code).IsUnique();
            entity.Property(c => c.Code).HasMaxLength(20).IsRequired();
            entity.Property(c => c.Title).HasMaxLength(150).IsRequired();
            entity.Property(c => c.Description).HasMaxLength(500);
        });

        // 5. Cấu hình bảng Enrollment (bảng trung gian quan hệ N-N giữa Student và Course)
        modelBuilder.Entity<Enrollment>(entity =>
        {
            // Tránh 1 học sinh đăng ký trùng 1 môn học 2 lần
            entity.HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();

            entity.HasOne(e => e.Student)
                  .WithMany(s => s.Enrollments)
                  .HasForeignKey(e => e.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Course)
                  .WithMany(c => c.Enrollments)
                  .HasForeignKey(e => e.CourseId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
