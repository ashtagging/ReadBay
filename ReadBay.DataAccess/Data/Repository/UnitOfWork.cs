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
            BookType = new BookTypeRepository(_db);
            Product = new ProductRepository(_db);
            Company = new CompanyRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            SP_Call = new SP_Call(_db);
        }

        public ICategoryRepository Category { get; private set; }

        public IBookTypeRepository BookType { get; private set; }

        public IProductRepository Product { get; private set; }

        public ICompanyRepository Company { get; set; }

        public ISP_Call SP_Call{ get; private set; }

        public IApplicationUserRepository ApplicationUser { get; private set; }

        public void Dispose()
        {
            _db.Dispose();
        }

        // Changes we make in repository are not saved but are saved in each models repository in Update()
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
