using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Internal
{
    public interface IInternalPersonService
    {
        PersonDto GetPersonByUserId(long userId);
    }
}
