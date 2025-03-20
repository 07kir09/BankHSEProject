using System;
using System.Collections.Generic;

public class RepositoryProxy<T> : IRepository<T> where T : class
{
    private readonly IRepository<T> _realRepository;
    private readonly Dictionary<Guid, T> _cache;

    public RepositoryProxy(IRepository<T> realRepository)
    {
        if (realRepository == null)
            throw new ArgumentNullException(nameof(realRepository));

        _realRepository = realRepository;
        _cache = new Dictionary<Guid, T>();

        var all = _realRepository.GetAll();
        foreach (var entity in all)
        {
            var id = GetEntityId(entity);
            _cache[id] = entity;
        }
    }

    public void Add(T entity)
    {
        var id = GetEntityId(entity);
        _cache[id] = entity;
        _realRepository.Add(entity);
    }

    public void Update(T entity)
    {
        var id = GetEntityId(entity);
        _cache[id] = entity;
        _realRepository.Add(entity);
    }

    public T GetById(Guid id)
    {
        if (_cache.TryGetValue(id, out var entity))
            return entity;
        
        entity = _realRepository.GetById(id);
        if (entity != null)
        {
            _cache[id] = entity;
        }

        return entity;
    }

    public IList<T> GetAll()
    {
        return new List<T>(_cache.Values);
    }

    private Guid GetEntityId(T entity)
    {
        var prop = entity.GetType().GetProperty("Id");
        return (Guid)prop.GetValue(entity);
    }
}