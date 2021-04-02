using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadBay.DataAccess.Data.Repository.IRepository
{
    // This is a generic repository in which the type of object (T) is not known
    public interface IRepository<T> where T: class
    {

    }
}
