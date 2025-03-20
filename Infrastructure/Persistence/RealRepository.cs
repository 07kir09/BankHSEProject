using System;
using System.Collections.Generic;
using System.Linq;

public class RealRepository<T> : IRepository<T> where T : class
{
    private readonly Dictionary<Guid, T> _storage = new Dictionary<Guid, T>();

    public void Add(T entity)
    {
        var id = GetEntityId(entity);
        _storage[id] = entity;
    }

    public void Update(T entity)
    {
        var id = GetEntityId(entity);
        _storage[id] = entity;
    }

    public T GetById(Guid id)
    {
        _storage.TryGetValue(id, out var entity);
        return entity;
    }

    public IList<T> GetAll()
    {
        // Даже если база пуста, возвращаем пустой список, а не null.
        return _storage.Values.ToList();
    }

    private Guid GetEntityId(T entity)
    {
        var prop = entity.GetType().GetProperty("Id");
        if (prop == null)
            throw new Exception("Свойство 'Id' не найдено у объекта.");
        return (Guid)prop.GetValue(entity);
    }
}