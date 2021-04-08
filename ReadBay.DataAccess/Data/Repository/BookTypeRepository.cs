using ReadBay.DataAccess.Data.Repository.IRepository;
using ReadBay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadBay.DataAccess.Data.Repository
{
    public class BookTypeRepository : Repository<BookType>, IBookTypeRepository
    {
        
        private readonly ApplicationDbContext _db;

        public BookTypeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(BookType bookType)
        {
            // for an entity S the Id should match the BookType ID, for all those records retrieve them (only one)
            var objFromDb = _db.BookTypes.FirstOrDefault(s => s.Id == bookType.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = bookType.Name;                
            }
        }
    }
}
