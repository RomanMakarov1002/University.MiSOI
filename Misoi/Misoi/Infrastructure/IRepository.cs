using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Misoi.Infrastructure
{
    public interface IRepository<T>
    {
        List<T> GetAll();
        void Add(T entity);
        void Delete(int id);
        void Update(T entity);
        T GetById(int id);
    }
}