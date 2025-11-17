using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.UseCases.Administration
{
    internal class ProblemService : IProblemService
    {
        private readonly IProblemRepository _problemRepository;
        private readonly IMapper _mapper;

        public ProblemService(IProblemRepository repository, IMapper mapper)
        {
            _problemRepository = repository;
            _mapper = mapper;
        }

        public PagedResult<ProblemDto> GetPaged(int page, int pageSize)
        {
            var result = _problemRepository.GetPaged(page, pageSize);

            var items = result.Results.Select(_mapper.Map<ProblemDto>).ToList();
            return new PagedResult<ProblemDto>(items, result.TotalCount);
        }

        public EquipmentDto Create(ProblemDto entity)
        {
            var result = _problemRepository.Create(_mapper.Map<Problem>(entity));
            return _mapper.Map<ProblemDto>(result);
        }

        public ProblemDto Update(ProblemDto entity)
        {
            var result = _problemRepository.Update(_mapper.Map<Equipment>(entity));
            return _mapper.Map<ProblemDto>(result);
        }

        public void Delete(long id)
        {
            _problemRepository.Delete(id);
        }
    }
}
