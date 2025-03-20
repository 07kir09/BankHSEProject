using System;
using System.Collections.Generic;
using System.Linq;

public interface IRepository<T>
{
    void Add(T entity);
    void Update(T entity);
    T GetById(Guid id);
    IList<T> GetAll();
}