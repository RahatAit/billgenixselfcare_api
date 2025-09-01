using billgenixselfcare_api.Domain.Common;

namespace billgenixselfcare_api.Domain.Entities
{
    public class Menu : BaseEntity
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int? ParentId { get; set; }
    }
}
