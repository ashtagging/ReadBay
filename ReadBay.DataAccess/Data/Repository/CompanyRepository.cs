using ReadBay.DataAccess.Data.Repository.IRepository;
using ReadBay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadBay.DataAccess.Data.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        
        private readonly ApplicationDbContext _db;

        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Company company)
        {
            // Instead of updating individual properties (See Category repository) we can simply use _db.Update()
            _db.Update(company);
        }
    }
}
