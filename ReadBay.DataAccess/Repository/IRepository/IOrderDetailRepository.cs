using System;
using ReadBay.Models;
using System.Collections.Generic;
using System.Text;
using ReadBay.DataAccess.Repository.IRepository;

namespace ReadBay.DataAccess.Repository.IRepository
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        void Update(OrderDetail obj);
    }
}
