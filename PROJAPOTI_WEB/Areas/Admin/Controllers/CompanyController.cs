using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PROJAPOTI.DataAccess.Repository.IRepository;
using PROJAPOTI.Models;
using PROJAPOTI.Models.ViewModels;
using PROJAPOTI.Utility;
using System.Data;

namespace PROJAPOTI_WEB.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
    
        public CompanyController(IUnitOfWork db)
        {
            _unitOfWork = db;
        }

        public IActionResult Index()
        {
            List<Company> obj = _unitOfWork.Company.GetAll().ToList();
            return View(obj);
        }
        [HttpGet]
        public IActionResult Upsert(int? id)
        {

            if(id==null || id == 0)
            {
                return View(new Company());
            }
            else
            {
                Company companyObj=_unitOfWork.Company.Get(u=>u.Id==id);
                return View(companyObj) ;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company companyObj)
        {
            if(ModelState.IsValid)
            {
             
                if (companyObj.Id == 0)
                {
                    _unitOfWork.Company.Add(companyObj);
                    _unitOfWork.save();
                    TempData["success"] = "Company created successfully!";
                }
                else
                {
                    _unitOfWork.Company.Update(companyObj);
                    _unitOfWork.save();
                    TempData["success"] = "Company updated successfully!";
                }

                return RedirectToAction("Index");
            }
            else
            {         
                return View(companyObj);
            }


        }

        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    var product = _unitOfWork.Company.Get(u => u.Id == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(product);
        //}

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public IActionResult DeletePost(int? id)
        //{
        //    var product = _unitOfWork.Company.Get(u => u.Id == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    _unitOfWork.Company.Remove(product);
        //    _unitOfWork.save();
        //    TempData["success"] = "Company delated successfully!";
        //    return RedirectToAction("Index");

        //}

        #region API CALLs
        [HttpGet]
        public IActionResult GetAll()
        {
            var product = _unitOfWork.Company.GetAll();
            return Json(new { data = product });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return Json(new { success = false, message = "Error while deleteing" });
            }
            var product = _unitOfWork.Company.Get(u => u.Id == id);
            if (product == null)
            {
                return Json(new { success = false, message = "Error while deleteing" });
            }
            _unitOfWork.Company.Remove(product);
            _unitOfWork.save();
            return Json(new { success = true, message = "Deleted successfully" });
        }
        #endregion 
    }
}
