using StudentManagement.Api.DTOs.Common;
using StudentManagement.Api.DTOs.Students;
using StudentManagement.Api.Entities;
using StudentManagement.Api.Repositories.Interfaces;
using StudentManagement.Api.Services.Interfaces;

namespace StudentManagement.Api.Services.Implementations;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IClassRepository _classRepository;

    public StudentService(IStudentRepository studentRepository, IClassRepository classRepository)
    {
        _studentRepository = studentRepository;
        _classRepository = classRepository;
    }

    public async Task<PagedResult<StudentDto>> GetStudentsAsync(StudentQueryParameters query)
    {
        var (items, totalCount) = await _studentRepository.GetPagedAsync(query);

        var dtos = items.Select(s => new StudentDto
        {
            Id = s.Id,
            StudentCode = s.StudentCode,
            FullName = s.FullName,
            DateOfBirth = s.DateOfBirth,
            Gender = s.Gender,
            Email = s.Email,
            PhoneNumber = s.PhoneNumber,
            Address = s.Address,
            ClassRoomId = s.ClassRoomId,
            ClassRoomName = s.ClassRoom != null ? s.ClassRoom.Name : null,
            EnrolledCoursesCount = s.Enrollments.Count
        }).ToList();

        return new PagedResult<StudentDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = query.PageNumber > 0 ? query.PageNumber : 1,
            PageSize = query.PageSize > 0 ? query.PageSize : 10
        };
    }

    public async Task<StudentDetailDto> GetStudentByIdAsync(int id)
    {
        var s = await _studentRepository.GetByIdWithDetailsAsync(id);
        if (s == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy học sinh với ID = {id}");
        }

        return new StudentDetailDto
        {
            Id = s.Id,
            StudentCode = s.StudentCode,
            FullName = s.FullName,
            DateOfBirth = s.DateOfBirth,
            Gender = s.Gender,
            Email = s.Email,
            PhoneNumber = s.PhoneNumber,
            Address = s.Address,
            ClassRoomId = s.ClassRoomId,
            ClassRoomName = s.ClassRoom?.Name,
            EnrolledCoursesCount = s.Enrollments.Count,
            EnrolledCourses = s.Enrollments.Select(e => new StudentCourseDto
            {
                EnrollmentId = e.Id,
                CourseId = e.CourseId,
                CourseCode = e.Course?.Code ?? string.Empty,
                CourseTitle = e.Course?.Title ?? string.Empty,
                Credits = e.Course?.Credits ?? 0,
                EnrollmentDate = e.EnrollmentDate,
                Grade = e.Grade
            }).ToList()
        };
    }

    public async Task<StudentDto> CreateStudentAsync(CreateStudentDto dto)
    {
        if (await _studentRepository.ExistsByCodeAsync(dto.StudentCode.Trim()))
        {
            throw new BadHttpRequestException($"Mã học sinh '{dto.StudentCode}' đã tồn tại trong hệ thống.");
        }

        if (dto.ClassRoomId.HasValue && dto.ClassRoomId.Value > 0)
        {
            var classRoom = await _classRepository.GetByIdAsync(dto.ClassRoomId.Value);
            if (classRoom == null)
            {
                throw new BadHttpRequestException($"Lớp học được chọn (ID = {dto.ClassRoomId}) không tồn tại.");
            }
        }

        var student = new Student
        {
            StudentCode = dto.StudentCode.Trim().ToUpper(),
            FullName = dto.FullName.Trim(),
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender.Trim(),
            Email = dto.Email?.Trim(),
            PhoneNumber = dto.PhoneNumber?.Trim(),
            Address = dto.Address?.Trim(),
            ClassRoomId = dto.ClassRoomId
        };

        var created = await _studentRepository.CreateAsync(student);
        var refreshed = await _studentRepository.GetByIdAsync(created.Id);

        return new StudentDto
        {
            Id = created.Id,
            StudentCode = created.StudentCode,
            FullName = created.FullName,
            DateOfBirth = created.DateOfBirth,
            Gender = created.Gender,
            Email = created.Email,
            PhoneNumber = created.PhoneNumber,
            Address = created.Address,
            ClassRoomId = created.ClassRoomId,
            ClassRoomName = refreshed?.ClassRoom?.Name,
            EnrolledCoursesCount = 0
        };
    }

    public async Task<StudentDto> UpdateStudentAsync(int id, UpdateStudentDto dto)
    {
        var student = await _studentRepository.GetByIdAsync(id);
        if (student == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy học sinh với ID = {id}");
        }

        if (dto.ClassRoomId.HasValue && dto.ClassRoomId.Value > 0)
        {
            var classRoom = await _classRepository.GetByIdAsync(dto.ClassRoomId.Value);
            if (classRoom == null)
            {
                throw new BadHttpRequestException($"Lớp học được chọn (ID = {dto.ClassRoomId}) không tồn tại.");
            }
        }

        student.FullName = dto.FullName.Trim();
        student.DateOfBirth = dto.DateOfBirth;
        student.Gender = dto.Gender.Trim();
        student.Email = dto.Email?.Trim();
        student.PhoneNumber = dto.PhoneNumber?.Trim();
        student.Address = dto.Address?.Trim();
        student.ClassRoomId = dto.ClassRoomId;

        var updated = await _studentRepository.UpdateAsync(student);
        var refreshed = await _studentRepository.GetByIdAsync(updated.Id);

        return new StudentDto
        {
            Id = updated.Id,
            StudentCode = updated.StudentCode,
            FullName = updated.FullName,
            DateOfBirth = updated.DateOfBirth,
            Gender = updated.Gender,
            Email = updated.Email,
            PhoneNumber = updated.PhoneNumber,
            Address = updated.Address,
            ClassRoomId = updated.ClassRoomId,
            ClassRoomName = refreshed?.ClassRoom?.Name,
            EnrolledCoursesCount = updated.Enrollments.Count
        };
    }

    public async Task<bool> DeleteStudentAsync(int id)
    {
        var existing = await _studentRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy học sinh với ID = {id}");
        }

        return await _studentRepository.DeleteAsync(id);
    }
}
