using AutoMapper;
using billgenixselfcare_api.Application.DTOs.Menu;
using billgenixselfcare_api.Application.Interfaces;
using billgenixselfcare_api.Domain.Common;
using billgenixselfcare_api.Domain.Entities;
using MediatR;

namespace billgenixselfcare_api.Application.Features.Menus
{
    public class CreateMenuCommand : IRequest<Result<MenuDto>>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int? ParentId { get; set; }
        public bool IsActive { get; set; }
        public string UserId { get; set; }
    }

    public class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommand, Result<MenuDto>>
    {
        private readonly IRepository<Menu> _repository;
        private readonly IMapper _mapper;

        public CreateMenuCommandHandler(IRepository<Menu> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<MenuDto>> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var model = new Menu
                {
                    Code = request.Code,
                    Name = request.Name,
                    Path = request.Path,
                    ParentId = request.ParentId,
                    IsActive = request.IsActive,
                    CreatedBy = request.UserId,
                };

                await _repository.AddAsync(model);
                var dto = _mapper.Map<MenuDto>(model);

                return Result<MenuDto>.SuccessResult(dto, "Created successfully");
            }
            catch (Exception ex)
            {
                return Result<MenuDto>.FailureResult("Failed to create", new List<string> { ex.Message });
            }
        }
    }
}
