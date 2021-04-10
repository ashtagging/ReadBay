using ReadBay.DataAccess.Data;
using ReadBay.DataAccess.Repository;
using ReadBay.DataAccess.Repository.IRepository;
using ReadBay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReadBay.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _db;

        public ShoppingCartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ShoppingCart obj)
        {
            _db.Update(obj);
        }
    }
}
