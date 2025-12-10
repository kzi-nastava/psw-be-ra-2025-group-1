using AutoMapper;
using Explorer.BuildingBlocks.Core.Domain;
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
    public class TransportTimeService : ITransportTimeService
    {
        private readonly ITransportTimeRepository _transportRepository;
        private readonly IMapper _mapper;
        public TransportTimeService(ITransportTimeRepository repository, IMapper mapper)
        {
            _transportRepository = repository;
            _mapper = mapper;
        }
        public TransportTimeDto Create(TransportTimeDto entity)
        {
            var result = _transportRepository.Create(_mapper.Map<TransportTime>(entity));
            return _mapper.Map<TransportTimeDto>(result);
        }

        public void Delete(long id)
        {
            _transportRepository.Delete(id);
        }

        public TransportTimeDto Get(long id)
        {
            return _mapper.Map<TransportTimeDto>(_transportRepository.Get(id));
        }

        public IEnumerable<TransportTimeDto> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TransportTimeDto> GetByTourId(long tourId)
        {
            var result = _transportRepository.GetByTourId(tourId);
            var items = result.Select(_mapper.Map<TransportTimeDto>).ToList();
            return items;
        }

        public IEnumerable<TransportTimeDto> GetByTransportType(TransportTypeDto transport)
        {
            var result = _transportRepository.GetByTransportType(MapTransportType(transport));
            var items = result.Select(_mapper.Map<TransportTimeDto>).ToList();
            return items;
        }

        public PagedResult<TransportTimeDto> GetPaged(int page, int pageSize)
        {
            var result = _transportRepository.GetPaged(page, pageSize);
            var items = result.Results.Select(_mapper.Map<TransportTimeDto>).ToList();
            return new PagedResult<TransportTimeDto>(items, result.TotalCount);
        }

        public TransportTimeDto Update(TransportTimeDto entity)
        {
            var result = _transportRepository.Update(_mapper.Map<TransportTime>(entity));
            return _mapper.Map<TransportTimeDto>(result);
        }

        private static TransportType MapTransportType(TransportTypeDto transport) => transport switch
        {
            TransportTypeDto.Car => TransportType.Car,
            TransportTypeDto.Bike => TransportType.Bike,
            TransportTypeDto.Foot => TransportType.Foot,
            TransportTypeDto.Unknown => TransportType.Unknown,
            _ => TransportType.Unknown,
        };
    }
}
