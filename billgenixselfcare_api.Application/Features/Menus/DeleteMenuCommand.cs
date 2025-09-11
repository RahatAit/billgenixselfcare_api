using billgenixselfcare_api.Application.Interfaces;
using billgenixselfcare_api.Domain.Common;
using billgenixselfcare_api.Domain.Entities;
using MediatR;

namespace billgenixselfcare_api.Application.Features.Menus
{
    public class DeleteMenuCommand : IRequest<Result>
    {
        public int Id { get; set; }
        public string UserId { get; set; }
    }

    public class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommand, Result>
    {
        private readonly IRepository<Menu> _repository;

        public DeleteMenuCommandHandler(IRepository<Menu> repository)
        {
            _repository = repository;
        }

        public async Task<Result> Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _repository.GetByIdAsync(request.Id);
                if (data == null)
                {
                    return Result.FailureResult("Not found");
                }

                data.IsActive = false;
                data.IsDeleted = true;
                data.DeletedBy = request.UserId;
                data.DateledAt = DateTime.UtcNow;
                await _repository.UpdateAsync(data);

                return Result.SuccessResult("Deleted successfully");
            }
            catch (Exception ex)
            {
                return Result.FailureResult("Failed to delete", new List<string> { ex.Message });
            }
        }
    }
}
