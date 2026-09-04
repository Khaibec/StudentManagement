namespace StudentApi.DTOs;

public class ClassDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CreateClassDto
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateClassDto
{
    public string Name { get; set; } = string.Empty;
}