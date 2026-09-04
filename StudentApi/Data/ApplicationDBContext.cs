using Microsoft.EntityFrameworkCore;
using StudentApi.Models;

namespace StudentApi.Data;

public class ApplicationDBContext : DbContext
{
    public ApplicationDBContext(
        DbContextOptions<ApplicationDBContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }

    public DbSet<Class> Classes { get; set; }

    public DbSet<Course> Courses { get; set; }

    public DbSet<Enrollment> Enrollments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Quan trọng: Đặt khóa chính cho bảng Enrollment
        builder.Entity<Enrollment>()
            .HasKey(e => new { e.StudentId, e.CourseId });

        // Các quan hệ giữa các bảng (ko cần thiết lắm do EF Core có thể tự động nhận ra, nhưng để rõ ràng hơn)
        builder.Entity<Student>()
            .HasOne(s => s.Class)
            .WithMany(c => c.Students)
            .HasForeignKey(s => s.ClassId);

        builder.Entity<Enrollment>()
            .HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId);

        builder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId);
    }
}
