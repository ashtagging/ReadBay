using ReadBay.DataAccess.Data.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadBay.DataAccess.Data.Repository
{
    // Not accessible unless added to configure services in startup.cs
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            SP_Call = new SP_Call(_db);
        }
        public ICategoryRepository Category { get; private set; }
        public ISP_Call SP_Call{ get; private set; }

        public void Dispose()
        {
            _db.Dispose();
        }

        // Not saving any changes we make in rpeository but are saving changes in each models repository in Update()
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
