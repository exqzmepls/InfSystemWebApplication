using InfSystemWebApplication.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace InfSystemWebApplication.Repositories
{
    public class Repository : IRepository
    {
        private InfSystemContext db;
        public Repository()
        {
            db = new InfSystemContext();
        }
        public List<T> GetList<T>() where T : class
        {
            return db.Set<T>().ToList();
        }
        public T Find<T>(params object[] keyValues) where T : class
        {
            return db.Set<T>().Find(keyValues);
        }

        public void Add<T>(T item) where T : class
        {
            db.Set<T>().Add(item);
        }

        public void Update<T>(T item) where T : class
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void Remove<T>(T item) where T : class
        {
            db.Set<T>().Remove(item);
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}