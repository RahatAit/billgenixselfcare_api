using billgenixselfcare_api.Domain.Common;
using billgenixselfcare_api.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace billgenixselfcare_api.Application.Services.Users
{
    public class DeleteUserCommand : IRequest<Result>
    {
        public string Id { get; set; }
        public string UserId { get; set; }
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

                data.IsActive = false;
                data.IsDeleted = true;
                data.DeletedBy = request.UserId;
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
