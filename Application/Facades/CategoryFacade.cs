using System;
using System.Collections.Generic;
using AccountingForFinances;

public class CategoryFacade
{
    public readonly IRepository<Category> _catedoryRepository;

    public CategoryFacade(IRepository<Category> catedoryRepository)
    {
        _catedoryRepository = catedoryRepository;
    }
    
    public Category CreateCategory(FinanceType type, string name)
    {
        var category = DomainFactory.CreateCategory(type, name);
        _catedoryRepository.Add(category);
        return category;
    }

    public Category GetCategory(Guid id)
    {
        return _catedoryRepository.GetById(id);
    }

    public IList<Category> GetAllCategories()
    {
        return _catedoryRepository.GetAll();
    }

    public void DeleteCategory(Guid id)
    {
        
    }
}