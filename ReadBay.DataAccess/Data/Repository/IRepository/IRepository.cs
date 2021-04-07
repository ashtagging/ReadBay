using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ReadBay.DataAccess.Data.Repository.IRepository
{
    // This is a generic repository in which the type of object (T) is not known
    public interface IRepository<T> where T: class
    {
        T Get(int id);

        // returns all 
        IEnumerable<T> GetAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null
            );

        // Returns 1 object
        T GetFirstOrDefault(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = null
            );

        void Add(T entity);

        //Remove an object or a complete entity, only 1 neccessary?
        void Remove(int id);

        void Remove(T entity);


        void RemoveRange(IEnumerable<T> entity);
    }
}
