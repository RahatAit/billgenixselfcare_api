using billgenixselfcare_api.Domain.Common;
using billgenixselfcare_api.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace billgenixselfcare_api.Application.Features.Users
{
    public class DeleteUserCommand : IRequest<Result>
    {
        public string Id { get; set; }
        public string DeleteBy { get; set; }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DeleteUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _userManager.FindByIdAsync(request.Id);
                if (data == null)
                {
                    return Result.FailureResult("Not found");
                }
                if (request.Id == request.DeleteBy)
                {
                    return Result.FailureResult("You cannot delete your own account");
                }

                //data.IsActive = false;
                data.IsDeleted = true;
                data.DeletedBy = request.DeleteBy;
                data.DateledAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(data);

                return Result.SuccessResult("Deleted successfully");
            }
            catch (Exception ex)
            {
                return Result.FailureResult("Failed to delete", new List<string> { ex.Message });
            }
        }
    }
}
