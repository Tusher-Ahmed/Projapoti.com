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
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _db;
        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Company company)
        {
           _db.Companies.Update(company);
        }
    }
}
