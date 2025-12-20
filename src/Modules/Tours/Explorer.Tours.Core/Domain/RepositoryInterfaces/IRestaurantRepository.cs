using System.Collections.Generic;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IRestaurantRepository
{
    List<Restaurant> GetAll();
    Restaurant Get(long id);
    Restaurant Create(Restaurant restaurant);
}
