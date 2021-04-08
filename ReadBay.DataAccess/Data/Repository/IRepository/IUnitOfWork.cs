using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadBay.DataAccess.Data.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }

        IBookTypeRepository BookType { get; }

        IProductRepository Product { get; }

        ISP_Call SP_Call { get; }

        void Save();
    }
}
