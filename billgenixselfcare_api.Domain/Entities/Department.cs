using billgenixselfcare_api.Domain.Common;

namespace billgenixselfcare_api.Domain.Entities
{
    public class Department : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
