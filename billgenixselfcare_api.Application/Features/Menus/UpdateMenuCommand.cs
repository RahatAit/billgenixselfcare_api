using AutoMapper;
using billgenixselfcare_api.Application.DTOs.Menu;
using billgenixselfcare_api.Application.Interfaces;
using billgenixselfcare_api.Domain.Common;
using billgenixselfcare_api.Domain.Entities;
using MediatR;

namespace billgenixselfcare_api.Application.Features.Menus
{
    public class UpdateMenuCommand : IRequest<Result<MenuDto>>
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int? ParentId { get; set; }
        public bool IsActive { get; set; }
        public string UserId { get; set; }
    }

    public class UpdateMenuCommandHandler : IRequestHandler<UpdateMenuCommand, Result<MenuDto>>
    {
        private readonly IRepository<Menu> _repository;
        private readonly IMapper _mapper;

        public UpdateMenuCommandHandler(IRepository<Menu> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<MenuDto>> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _repository.GetByIdAsync(request.Id);
                if (data == null)
                {
                    return Result<MenuDto>.FailureResult("Not found");
                }

                data.Code = request.Code;
                data.Name = request.Name;
                data.Path = request.Path;
                data.ParentId = request.ParentId;
                data.IsActive = request.IsActive;
                data.UpdatedBy = request.UserId;
                data.UpdatedAt = DateTime.UtcNow;

                await _repository.UpdateAsync(data);
                var dto = _mapper.Map<MenuDto>(data);

                return Result<MenuDto>.SuccessResult(dto, "Updated successfully");
            }
            catch (Exception ex)
            {
                return Result<MenuDto>.FailureResult("Failed to update", new List<string> { ex.Message });
            }
        }
    }
}
