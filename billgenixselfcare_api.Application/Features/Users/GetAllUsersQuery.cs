using billgenixselfcare_api.Application.DTOs;
using billgenixselfcare_api.Application.Interfaces;
using billgenixselfcare_api.Domain.Common;
using MediatR;

namespace billgenixselfcare_api.Application.Features.Users
{
    public class GetAllUsersQuery : IRequest<Result<PaginatedList<UserDto>>>
    {
        public string Search { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string RoleId { get; set; }
        public bool? IsActive { get; set; }
    }

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<PaginatedList<UserDto>>>
    {
        private readonly IUserRepository _repository;

        public GetAllUsersQueryHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<PaginatedList<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _repository.GetAllAsync(request.Search, request.PageNumber ?? 1, request.PageSize ?? 10, request.RoleId, request.IsActive);
                return Result<PaginatedList<UserDto>>.SuccessResult(new PaginatedList<UserDto>(data.Items, data.TotalCount, data.PageIndex, data.PageSize));
            }
            catch (Exception ex)
            {
                return Result<PaginatedList<UserDto>>.FailureResult("Failed to retrieve", new List<string> { ex.Message });
            }
        }
    }
}
