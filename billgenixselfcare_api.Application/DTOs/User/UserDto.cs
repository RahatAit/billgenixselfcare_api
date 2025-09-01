using billgenixselfcare_api.Domain.Common;

namespace billgenixselfcare_api.Application.DTOs.User
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime? DOB { get; set; }
        public Gender? Gender { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Roles { get; set; }
    }
}
