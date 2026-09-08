using StudentManagement.Api.Entities;

namespace StudentManagement.Api.Data;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        // Đảm bảo database đã được tạo
        context.Database.EnsureCreated();

        // 1. Seed Users nếu chưa có
        if (!context.Users.Any())
        {
            var users = new List<User>
            {
                new User
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    FullName = "Quản Trị Viên",
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Username = "user",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
                    FullName = "Người Dùng Thường",
                    Role = "User",
                    CreatedAt = DateTime.UtcNow
                }
            };
            context.Users.AddRange(users);
            context.SaveChanges();
        }

        // 2. Seed Classes nếu chưa có
        if (!context.Classes.Any())
        {
            var classes = new List<ClassRoom>
            {
                new ClassRoom { Name = "10A1", Description = "Lớp 10 Chuyên Toán" },
                new ClassRoom { Name = "11B2", Description = "Lớp 11 Chuyên Lý" },
                new ClassRoom { Name = "12C3", Description = "Lớp 12 Chuyên Hóa" }
            };
            context.Classes.AddRange(classes);
            context.SaveChanges();
        }

        // 3. Seed Courses nếu chưa có
        if (!context.Courses.Any())
        {
            var courses = new List<Course>
            {
                new Course { Code = "MATH101", Title = "Toán Cao Cấp 1", Credits = 3, Description = "Giải tích và Đại số tuyến tính căn bản" },
                new Course { Code = "PROG101", Title = "Lập trình C# Cơ Bản", Credits = 4, Description = "Lập trình hướng đối tượng với C# và .NET" },
                new Course { Code = "DB101", Title = "Cơ Sở Dữ Liệu SQL", Credits = 3, Description = "Thiết kế CSDL quan hệ và truy vấn T-SQL" },
                new Course { Code = "ENG101", Title = "Tiếng Anh Chuyên Ngành", Credits = 2, Description = "Từ vựng và kỹ năng đọc tài liệu CNTT" }
            };
            context.Courses.AddRange(courses);
            context.SaveChanges();
        }

        // 4. Seed Students nếu chưa có
        if (!context.Students.Any())
        {
            var classList = context.Classes.ToList();
            var class10A1 = classList.FirstOrDefault(c => c.Name == "10A1");
            var class11B2 = classList.FirstOrDefault(c => c.Name == "11B2");
            var class12C3 = classList.FirstOrDefault(c => c.Name == "12C3");

            var students = new List<Student>
            {
                new Student
                {
                    StudentCode = "SV001",
                    FullName = "Nguyễn Văn An",
                    DateOfBirth = new DateTime(2008, 1, 15),
                    Gender = "Nam",
                    Email = "an.nguyen@example.com",
                    PhoneNumber = "0901234567",
                    Address = "Hà Nội",
                    ClassRoomId = class10A1?.Id
                },
                new Student
                {
                    StudentCode = "SV002",
                    FullName = "Trần Thị Mai",
                    DateOfBirth = new DateTime(2008, 5, 20),
                    Gender = "Nữ",
                    Email = "mai.tran@example.com",
                    PhoneNumber = "0902345678",
                    Address = "Đà Nẵng",
                    ClassRoomId = class10A1?.Id
                },
                new Student
                {
                    StudentCode = "SV003",
                    FullName = "Lê Hoàng Nam",
                    DateOfBirth = new DateTime(2007, 9, 10),
                    Gender = "Nam",
                    Email = "nam.le@example.com",
                    PhoneNumber = "0903456789",
                    Address = "TP. Hồ Chí Minh",
                    ClassRoomId = class11B2?.Id
                },
                new Student
                {
                    StudentCode = "SV004",
                    FullName = "Phạm Thu Hà",
                    DateOfBirth = new DateTime(2007, 11, 25),
                    Gender = "Nữ",
                    Email = "ha.pham@example.com",
                    PhoneNumber = "0904567890",
                    Address = "Hải Phòng",
                    ClassRoomId = class11B2?.Id
                },
                new Student
                {
                    StudentCode = "SV005",
                    FullName = "Vũ Quốc Bảo",
                    DateOfBirth = new DateTime(2006, 3, 8),
                    Gender = "Nam",
                    Email = "bao.vu@example.com",
                    PhoneNumber = "0905678901",
                    Address = "Cần Thơ",
                    ClassRoomId = class12C3?.Id
                }
            };
            context.Students.AddRange(students);
            context.SaveChanges();
        }

        // 5. Seed Enrollments nếu chưa có
        if (!context.Enrollments.Any())
        {
            var students = context.Students.ToList();
            var courses = context.Courses.ToList();

            var sv1 = students.FirstOrDefault(s => s.StudentCode == "SV001");
            var sv2 = students.FirstOrDefault(s => s.StudentCode == "SV002");
            var sv3 = students.FirstOrDefault(s => s.StudentCode == "SV003");
            var sv4 = students.FirstOrDefault(s => s.StudentCode == "SV004");
            var sv5 = students.FirstOrDefault(s => s.StudentCode == "SV005");

            var math = courses.FirstOrDefault(c => c.Code == "MATH101");
            var prog = courses.FirstOrDefault(c => c.Code == "PROG101");
            var db = courses.FirstOrDefault(c => c.Code == "DB101");
            var eng = courses.FirstOrDefault(c => c.Code == "ENG101");

            var enrollments = new List<Enrollment>();

            if (sv1 != null && math != null) enrollments.Add(new Enrollment { StudentId = sv1.Id, CourseId = math.Id, Grade = 8.5 });
            if (sv1 != null && prog != null) enrollments.Add(new Enrollment { StudentId = sv1.Id, CourseId = prog.Id, Grade = 9.0 });
            if (sv2 != null && math != null) enrollments.Add(new Enrollment { StudentId = sv2.Id, CourseId = math.Id, Grade = 7.5 });
            if (sv3 != null && db != null) enrollments.Add(new Enrollment { StudentId = sv3.Id, CourseId = db.Id, Grade = 8.0 });
            if (sv4 != null && eng != null) enrollments.Add(new Enrollment { StudentId = sv4.Id, CourseId = eng.Id, Grade = null });
            if (sv5 != null && prog != null) enrollments.Add(new Enrollment { StudentId = sv5.Id, CourseId = prog.Id, Grade = 8.8 });

            context.Enrollments.AddRange(enrollments);
            context.SaveChanges();
        }
    }
}
