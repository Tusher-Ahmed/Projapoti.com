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
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork db, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> obj = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return View(obj);
        }
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString(),
            });
            ProductVM productVM = new()
            {
                CategoryList = CategoryList,
                Product = new Product()
            };
            if(id==null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product=_unitOfWork.Product.Get(u=>u.Id==id);
                return View(productVM) ;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if(ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\products");
                    if(!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        var oldImagePath=Path.Combine(wwwRootPath,productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\products\" + fileName;
                    ;
                }
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                    _unitOfWork.save();
                    TempData["success"] = "Product created successfully!";
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                    _unitOfWork.save();
                    TempData["success"] = "Product updated successfully!";
                }

                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                });
                return View(productVM);
            }


        }

        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    var product = _unitOfWork.Product.Get(u => u.Id == id);
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
        //    var product = _unitOfWork.Product.Get(u => u.Id == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    _unitOfWork.Product.Remove(product);
        //    _unitOfWork.save();
        //    TempData["success"] = "Product delated successfully!";
        //    return RedirectToAction("Index");

        //}

        #region API CALLs
        [HttpGet]
        public IActionResult GetAll()
        {
            var product = _unitOfWork.Product.GetAll(includeProperties: "Category");
            return Json(new { data = product });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return Json(new { success = false, message = "Error while deleteing" });
            }
            var product = _unitOfWork.Product.Get(u => u.Id == id);
            if (product == null)
            {
                return Json(new { success = false, message = "Error while deleteing" });
            }
            //deleting image from images
            var oldImage = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImage))
            {
                System.IO.File.Delete(oldImage);
            }
            _unitOfWork.Product.Remove(product);
            _unitOfWork.save();
            return Json(new { success = true, message = "Deleted successfully" });
        }
        #endregion 
    }
}
