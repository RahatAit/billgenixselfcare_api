using billgenixselfcare_api.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace billgenixselfcare_api.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public DateTime? DOB { get; set; }
        public Gender? Gender { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string DeletedBy { get; set; }
        public DateTime? DateledAt { get; set; }
    }
}
