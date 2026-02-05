using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Internal;
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
    public class PersonService : IPersonService, IInternalPersonService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IMapper _mapper;

        public PersonService(IPersonRepository personRepository, IMapper mapper)
        {
            _personRepository = personRepository;
            _mapper = mapper;
        }

        public PersonDto Update(long personId, PersonDto entity)
        {
            var person = _personRepository.Get(personId);
            if (person == null)
            {
                throw new KeyNotFoundException($"Person with id {personId} not found.");
            }

            person.Update(
                entity.Name,
                entity.Surname,
                entity.Email,
                entity.ProfileImageUrl,
                entity.Biography,
                entity.Quote);

            var result = _personRepository.Update(person);
            return _mapper.Map<PersonDto>(result);
        }

        public PersonDto Get(long id)
        {
            var result = _personRepository.Get(id);
            return _mapper.Map<PersonDto>(result);
        }

        public PersonDto GetPersonByUserId(long userId)
        {
            var person = _personRepository.GetByUserId(userId);
            return _mapper.Map<PersonDto>(person);
        }
    }
}
