using Moq;
using StudentManagement.Api.DTOs.Students;
using StudentManagement.Api.Entities;
using StudentManagement.Api.Exceptions;
using StudentManagement.Api.Repositories.Interfaces;
using StudentManagement.Api.Services.Implementations;
using Xunit;

namespace StudentManagement.Tests.Services;


/// Unit Tests cho StudentService sử dụng xUnit và Moq
/// Kiểm thử logic nghiệp vụ mà không cần kết nối tới CSDL thật 

public class StudentServiceTests
{
    private readonly Mock<IStudentRepository> _studentRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IClassRepository> _classRepoMock;
    private readonly StudentService _studentService;

    public StudentServiceTests()
    {
        // 1. Mock thành công IStudentRepository và IUnitOfWork (kèm IClassRepository)
        _studentRepoMock = new Mock<IStudentRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _classRepoMock = new Mock<IClassRepository>();

        // Khởi tạo StudentService với các dependencies đã được mock
        _studentService = new StudentService(
            _studentRepoMock.Object,
            _unitOfWorkMock.Object,
            _classRepoMock.Object);
    }

    #region 1. Test Cases: Thêm học sinh (CreateStudentAsync)

    [Fact]
    public async Task CreateStudentAsync_WhenStudentCodeAlreadyExists_ShouldThrowDuplicateStudentCodeException()
    {
        // Arrange: Giả lập tình huống mã sinh viên 'SV001' đã tồn tại trong repository
        var createDto = new CreateStudentDto
        {
            StudentCode = "SV001",
            FullName = "Nguyen Van A",
            DateOfBirth = new DateTime(2005, 1, 1),
            Gender = "Nam"
        };

        _studentRepoMock
            .Setup(repo => repo.ExistsByCodeAsync("SV001", null))
            .ReturnsAsync(true);

        // Act & Assert: Service phát hiện trùng và ném ra lỗi nghiệp vụ DuplicateStudentCodeException
        var exception = await Assert.ThrowsAsync<DuplicateStudentCodeException>(() =>
            _studentService.CreateStudentAsync(createDto));

        Assert.Equal("SV001", exception.StudentCode);
        Assert.Contains("đã tồn tại", exception.Message);

        // Đảm bảo không gọi repository lưu học sinh và không gọi UnitOfWork lưu giao dịch
        _studentRepoMock.Verify(repo => repo.CreateAsync(It.IsAny<Student>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateStudentAsync_WhenValidDto_ShouldCallRepositoryAndUnitOfWorkSuccessfully()
    {
        // Arrange: Giả lập dữ liệu thêm mới hợp lệ và mã sinh viên chưa tồn tại
        var createDto = new CreateStudentDto
        {
            StudentCode = "SV002",
            FullName = "Tran Thi B",
            DateOfBirth = new DateTime(2005, 5, 10),
            Gender = "Nữ",
            Email = "thib@gmail.com",
            PhoneNumber = "0987654321"
        };

        var savedStudent = new Student
        {
            Id = 10,
            StudentCode = "SV002",
            FullName = "Tran Thi B",
            DateOfBirth = new DateTime(2005, 5, 10),
            Gender = "Nữ",
            Email = "thib@gmail.com",
            PhoneNumber = "0987654321"
        };

        _studentRepoMock
            .Setup(repo => repo.ExistsByCodeAsync("SV002", null))
            .ReturnsAsync(false);

        _studentRepoMock
            .Setup(repo => repo.CreateAsync(It.IsAny<Student>()))
            .ReturnsAsync(savedStudent);

        _unitOfWorkMock
            .Setup(uow => uow.CompleteAsync())
            .ReturnsAsync(1);

        _studentRepoMock
            .Setup(repo => repo.GetByIdAsync(savedStudent.Id))
            .ReturnsAsync(savedStudent);

        // Act
        var result = await _studentService.CreateStudentAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal("SV002", result.StudentCode);
        Assert.Equal("Tran Thi B", result.FullName);

        // Xác minh repository CreateAsync được gọi đúng 1 lần với đúng mã sinh viên
        _studentRepoMock.Verify(repo => repo.CreateAsync(It.Is<Student>(s => s.StudentCode == "SV002")), Times.Once);

        // Xác minh UnitOfWork CompleteAsync được gọi đúng 1 lần để hoàn tất giao dịch
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    #endregion

    #region 2. Test Cases: Cập nhật học sinh (UpdateStudentAsync)

    [Fact]
    public async Task UpdateStudentAsync_WhenStudentNotFound_ShouldThrowStudentNotFoundException()
    {
        // Arrange: Giả lập repository không tìm thấy học sinh theo ID (trả về null mà không cần query SQL Server)
        int nonExistentId = 999;
        var updateDto = new UpdateStudentDto
        {
            FullName = "Nguyen Van B",
            DateOfBirth = new DateTime(2005, 5, 5),
            Gender = "Nam"
        };

        _studentRepoMock
            .Setup(repo => repo.GetByIdAsync(nonExistentId))
            .ReturnsAsync((Student?)null);

        // Act & Assert: Service ném ra StudentNotFoundException
        var exception = await Assert.ThrowsAsync<StudentNotFoundException>(() =>
            _studentService.UpdateStudentAsync(nonExistentId, updateDto));

        Assert.Equal(nonExistentId, exception.StudentId);
        Assert.Contains(nonExistentId.ToString(), exception.Message);

        // Xác minh không gọi repository UpdateAsync và không gọi UnitOfWork CompleteAsync
        _studentRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Student>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateStudentAsync_WhenStudentExists_ShouldUpdateAndCompleteUnitOfWork()
    {
        // Arrange: Giả lập học sinh tồn tại và cập nhật thông tin thành công
        int studentId = 1;
        var existingStudent = new Student
        {
            Id = studentId,
            StudentCode = "SV001",
            FullName = "Nguyen Van A",
            DateOfBirth = new DateTime(2005, 1, 1),
            Gender = "Nam",
            Email = "old@gmail.com"
        };

        var updateDto = new UpdateStudentDto
        {
            FullName = "Nguyen Van A (Cap Nhat)",
            DateOfBirth = new DateTime(2005, 1, 1),
            Gender = "Nam",
            Email = "new@gmail.com"
        };

        _studentRepoMock
            .Setup(repo => repo.GetByIdAsync(studentId))
            .ReturnsAsync(existingStudent);

        _studentRepoMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Student>()))
            .ReturnsAsync(existingStudent);

        _unitOfWorkMock
            .Setup(uow => uow.CompleteAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _studentService.UpdateStudentAsync(studentId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Nguyen Van A (Cap Nhat)", result.FullName);
        Assert.Equal("new@gmail.com", result.Email);

        // Xác minh repository UpdateAsync và UnitOfWork CompleteAsync được gọi đúng 1 lần
        _studentRepoMock.Verify(repo => repo.UpdateAsync(It.Is<Student>(s => s.Id == studentId)), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    #endregion

    #region 3. Test Cases: Xóa học sinh (DeleteStudentAsync)

    [Fact]
    public async Task DeleteStudentAsync_WhenStudentNotFound_ShouldThrowStudentNotFoundException()
    {
        // Arrange: Giả lập học sinh cần xóa không tồn tại (trả về null không cần query SQL Server)
        int nonExistentId = 999;

        _studentRepoMock
            .Setup(repo => repo.GetByIdAsync(nonExistentId))
            .ReturnsAsync((Student?)null);

        // Act & Assert: Service ném ra StudentNotFoundException
        var exception = await Assert.ThrowsAsync<StudentNotFoundException>(() =>
            _studentService.DeleteStudentAsync(nonExistentId));

        Assert.Equal(nonExistentId, exception.StudentId);
        Assert.Contains(nonExistentId.ToString(), exception.Message);

        // Xác minh không gọi repository DeleteAsync và không gọi UnitOfWork CompleteAsync
        _studentRepoMock.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteStudentAsync_WhenStudentExists_ShouldDeleteAndCompleteUnitOfWork()
    {
        // Arrange: Giả lập học sinh tồn tại và xóa thành công
        int studentId = 1;
        var existingStudent = new Student
        {
            Id = studentId,
            StudentCode = "SV001",
            FullName = "Nguyen Van A"
        };

        _studentRepoMock
            .Setup(repo => repo.GetByIdAsync(studentId))
            .ReturnsAsync(existingStudent);

        _studentRepoMock
            .Setup(repo => repo.DeleteAsync(studentId))
            .ReturnsAsync(true);

        _unitOfWorkMock
            .Setup(uow => uow.CompleteAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _studentService.DeleteStudentAsync(studentId);

        // Assert
        Assert.True(result);

        // Xác minh repository DeleteAsync và UnitOfWork CompleteAsync được gọi đúng 1 lần
        _studentRepoMock.Verify(repo => repo.DeleteAsync(studentId), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    #endregion
}
