using billgenixselfcare_api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace billgenixselfcare_api.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Department> Departments { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
