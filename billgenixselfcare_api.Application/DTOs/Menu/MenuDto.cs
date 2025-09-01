namespace billgenixselfcare_api.Application.DTOs.Menu
{
    public class MenuDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int? ParentId { get; set; }
    }
}
