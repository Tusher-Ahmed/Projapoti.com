using PROJAPOTI.DataAccess.Data;
using PROJAPOTI.DataAccess.Repository.IRepository;
using PROJAPOTI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJAPOTI.DataAccess.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>,IOrderDetailRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderDetailRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(OrderDetail orderDetail)
        {
           _db.OrderDetails.Update(orderDetail);
        }
    }
}
