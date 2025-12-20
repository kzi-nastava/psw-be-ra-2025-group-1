using System.Collections.Generic;
using System.Linq;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class RestaurantDbRepository : IRestaurantRepository
{
    private readonly ToursContext _dbContext;
    private readonly DbSet<Restaurant> _dbSet;

    public RestaurantDbRepository(ToursContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<Restaurant>();
    }

    public List<Restaurant> GetAll()
    {
        return _dbSet.ToList();
    }

    public Restaurant Get(long id)
    {
        var entity = _dbSet.Find(id);
        if (entity == null) throw new NotFoundException("Restaurant not found: " + id);
        return entity;
    }

    public Restaurant Create(Restaurant restaurant)
    {
        _dbSet.Add(restaurant);
        _dbContext.SaveChanges();
        return restaurant;
    }
}
