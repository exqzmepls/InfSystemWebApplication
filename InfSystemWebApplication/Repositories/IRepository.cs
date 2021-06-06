using System;
using System.Collections.Generic;

namespace InfSystemWebApplication.Repositories
{
    public interface IRepository : IDisposable 
    {
        List<T> GetList<T>() where T : class;
        T Find<T>(params object[] keyValues) where T : class;
        void Add<T>(T item) where T : class;
        void Update<T>(T item) where T : class;
        void Remove<T>(T item) where T : class;
        void SaveChanges();
    }
}
