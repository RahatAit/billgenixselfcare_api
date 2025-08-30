namespace billgenixselfcare_api.Domain.Common
{
    public abstract class BaseEntity
    {
        public bool IsActive { get; set; } = true;
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string DeletedBy { get; set; }
        public DateTime? DateledAt { get; set; }
    }
}
