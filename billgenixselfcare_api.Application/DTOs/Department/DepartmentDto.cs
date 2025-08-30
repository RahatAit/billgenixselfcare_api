namespace billgenixselfcare_api.Application.DTOs.Department
{
    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CreateDepartmentDto
    {
        public string Name { get; set; }
    }

    public class UpdateDepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
