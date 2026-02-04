using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPersonRepository _personRepository;

        public UserManagementService(IUserRepository userRepository, IPersonRepository personRepository)
        {
            _userRepository = userRepository;
            _personRepository = personRepository;
        }

        public List<AccountDto> GetAll()
        {
            var users = _userRepository.GetAll();

            return users.Select(user =>
            {
                Person person = null;
                try
                {
                    person = _personRepository.GetByUserId(user.Id);
                }
                catch (KeyNotFoundException) { }

                return new AccountDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = person?.Email ?? "",
                    Name = person?.Name ?? "",
                    Surname = person?.Surname ?? "",
                    Role = (AccountRole)user.Role,
                    IsActive = user.IsActive
                };
            }).ToList();
        }

        public void BlockUser(long userId)
        {
            var user = _userRepository.Get(userId);
            user.IsActive = false;
            _userRepository.Update(user);
        }

        public AccountDto GetByUsername(string username)
        {
            var user = _userRepository.GetActiveByName(username);
            var person = _personRepository.GetByUserId(user.Id);

            AccountDto account = new AccountDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = person?.Email ?? "",
                Name = person?.Name ?? "",
                Surname = person?.Surname ?? "",
                Role = (AccountRole)user.Role,
                IsActive = user.IsActive
            };

            return account;
        }

        public AccountDto GetById(long id)
        {
            var user = _userRepository.Get(id);
            var person = _personRepository.GetByUserId(user.Id);
            AccountDto account = new AccountDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = person?.Email ?? "",
                Name = person?.Name ?? "",
                Surname = person?.Surname ?? "",
                Role = (AccountRole)user.Role,
                IsActive = user.IsActive
            };
            return account;
        }
    }
}
