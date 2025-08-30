using billgenixselfcare_api.Domain.Common;

namespace billgenixselfcare_api.Application.DTOs.Auth
{
    public class RegisterDto
    {
        public string Name { get; set; }
        public DateTime? DOB { get; set; }
        public Gender? Gender { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
