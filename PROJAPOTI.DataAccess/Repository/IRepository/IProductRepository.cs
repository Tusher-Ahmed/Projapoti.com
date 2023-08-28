using PROJAPOTI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJAPOTI.DataAccess.Repository.IRepository
{
    public interface IProductRepository:IRepository<Product>
    {
        void Update(Product product);
    }
}
