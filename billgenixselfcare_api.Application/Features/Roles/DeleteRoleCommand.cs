using billgenixselfcare_api.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace billgenixselfcare_api.Application.Features.Roles
{
    public class DeleteRoleCommand : IRequest<Result>
    {
        public string Id { get; set; }
    }

    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Result>
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public DeleteRoleCommandHandler(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _roleManager.FindByIdAsync(request.Id);
                if (data == null)
                {
                    return Result.FailureResult("Not found");
                }
                await _roleManager.DeleteAsync(data);
                return Result.SuccessResult("Deleted successfully");
            }
            catch (Exception ex)
            {
                return Result.FailureResult("Failed to delete", new List<string> { ex.Message });
            }
        }
    }
}
