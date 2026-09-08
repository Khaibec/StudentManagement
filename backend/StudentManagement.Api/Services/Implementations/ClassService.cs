using StudentManagement.Api.DTOs.Classes;
using StudentManagement.Api.Entities;
using StudentManagement.Api.Repositories.Interfaces;
using StudentManagement.Api.Services.Interfaces;

namespace StudentManagement.Api.Services.Implementations;

public class ClassService : IClassService
{
    private readonly IClassRepository _classRepository;

    public ClassService(IClassRepository classRepository)
    {
        _classRepository = classRepository;
    }

    public async Task<IEnumerable<ClassDto>> GetAllClassesAsync()
    {
        var classes = await _classRepository.GetAllAsync();
        return classes.Select(c => new ClassDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            StudentCount = c.Students.Count
        });
    }

    public async Task<ClassDto> GetClassByIdAsync(int id)
    {
        var c = await _classRepository.GetByIdAsync(id);
        if (c == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy lớp học với mã ID = {id}");
        }

        return new ClassDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            StudentCount = c.Students.Count
        };
    }

    public async Task<ClassDto> CreateClassAsync(CreateClassDto dto)
    {
        if (await _classRepository.ExistsByNameAsync(dto.Name.Trim()))
        {
            throw new BadHttpRequestException($"Tên lớp '{dto.Name}' đã tồn tại trong hệ thống.");
        }

        var classRoom = new ClassRoom
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim()
        };

        var created = await _classRepository.CreateAsync(classRoom);

        return new ClassDto
        {
            Id = created.Id,
            Name = created.Name,
            Description = created.Description,
            StudentCount = 0
        };
    }

    public async Task<ClassDto> UpdateClassAsync(int id, UpdateClassDto dto)
    {
        var classRoom = await _classRepository.GetByIdAsync(id);
        if (classRoom == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy lớp học với mã ID = {id}");
        }

        if (await _classRepository.ExistsByNameAsync(dto.Name.Trim(), id))
        {
            throw new BadHttpRequestException($"Tên lớp '{dto.Name}' đã tồn tại trong hệ thống.");
        }

        classRoom.Name = dto.Name.Trim();
        classRoom.Description = dto.Description?.Trim();

        var updated = await _classRepository.UpdateAsync(classRoom);

        return new ClassDto
        {
            Id = updated.Id,
            Name = updated.Name,
            Description = updated.Description,
            StudentCount = updated.Students.Count
        };
    }

    public async Task<bool> DeleteClassAsync(int id)
    {
        var existing = await _classRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy lớp học với mã ID = {id}");
        }

        return await _classRepository.DeleteAsync(id);
    }
}
