using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IMapper _mapper;

        public PersonService(IPersonRepository personRepository, IMapper mapper)
        {
            _personRepository = personRepository;
            _mapper = mapper;
        }

        public PersonDto Update(PersonDto entity)
        {
            var result = _personRepository.Update(_mapper.Map<Person>(entity));
            return _mapper.Map<PersonDto>(result);
        }

        public PersonDto Get(long id)
        {
            var result = _personRepository.Get(id);
            return _mapper.Map<PersonDto>(result);
        }
    }
}
