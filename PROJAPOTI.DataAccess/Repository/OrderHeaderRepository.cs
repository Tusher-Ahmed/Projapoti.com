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
    public class OrderHeaderRepository : Repository<OrderHeader>,IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(OrderHeader orderHeader)
        {
           _db.OrderHeaders.Update(orderHeader);
        }

		public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
		{
			var orderFromDb=_db.OrderHeaders.FirstOrDefault(u=>u.Id == id);
            if(orderFromDb != null)
            {
                orderFromDb.OrderStatus= orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus= paymentStatus;
                }
            }
		}

		public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
		{
			var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (!string.IsNullOrEmpty(sessionId))
            {
                orderFromDb.SessionId=sessionId;
            }
            if(!string.IsNullOrEmpty(paymentIntentId))
            {
                orderFromDb.PaymentIntentId=paymentIntentId;
                orderFromDb.PaymentDate= DateTime.Now;
            }

		}
	}
}
