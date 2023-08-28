using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NuGet.DependencyResolver;
using PROJAPOTI.DataAccess.Repository.IRepository;
using PROJAPOTI.Models;
using PROJAPOTI.Models.ViewModels;
using PROJAPOTI.Utility;
using System.Diagnostics;
using System.Security.Claims;

namespace PROJAPOTI_WEB.Areas.Admin.Controllers
{
	[Area("Admin")]
	//[Authorize(Roles = SD.Role_Admin)]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		[BindProperty]
		private OrderVM OrderVM { get; set; }
		public OrderController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			return View();
		}
		public IActionResult Details(int orderId)
		{
			OrderVM = new()
			{
				orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
				orderDetails=_unitOfWork.OrderDetail.GetAll(u=>u.OrderHeaderId==orderId,includeProperties:"Product")
			};
			return View(OrderVM);
		}
		[HttpPost]
		[Authorize(Roles =SD.Role_Admin+","+SD.Role_Employee)]
		[ValidateAntiForgeryToken]
		public IActionResult UpdateOrderDetail(OrderVM orderVM )
		{
			var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == orderVM.orderHeader.Id);
			orderHeaderFromDb.Name = orderVM.orderHeader.Name;
			orderHeaderFromDb.PhoneNumber = orderVM.orderHeader.PhoneNumber;
			orderHeaderFromDb.StreetAddress = orderVM.orderHeader.StreetAddress;
			orderHeaderFromDb.Strate = orderVM.orderHeader.Strate;
			orderHeaderFromDb.City = orderVM.orderHeader.City;
			orderHeaderFromDb.PostalCode = orderVM.orderHeader.PostalCode;
			if (!string.IsNullOrEmpty(orderVM.orderHeader.Carrier))
			{
				orderHeaderFromDb.Carrier = orderVM.orderHeader.Carrier;
			}
            if (!string.IsNullOrEmpty(orderVM.orderHeader.TrackingNumber))
            {
                orderHeaderFromDb.Carrier = orderVM.orderHeader.TrackingNumber;
            }
			_unitOfWork.OrderHeader.Update(orderHeaderFromDb);
			_unitOfWork.save();
			TempData["success"] = "Order details updated successfully!";
            return RedirectToAction(nameof(Details), new {orderId=orderHeaderFromDb.Id});
		}
        #region API CALLs
        [HttpGet]		
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> orderHeaders;

			if(User.IsInRole(SD.Role_Admin)|| User.IsInRole(SD.Role_Employee))
			{
                orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
			}
			else
			{
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
				orderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.ApllicationUserId == userId, includeProperties: "ApplicationUser");

			}

            switch (status)
            {
                case "pending":
					orderHeaders = orderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
				case "inprocess":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusProcess);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }

            return Json(new { data = orderHeaders });
		}

		
		#endregion
	}
}
