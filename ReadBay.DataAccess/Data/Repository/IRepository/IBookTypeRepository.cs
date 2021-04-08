using ReadBay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadBay.DataAccess.Data.Repository.IRepository
{
    public interface IBookTypeRepository : IRepository<BookType>
    {
        void Update(BookType BookType);
    }
}
