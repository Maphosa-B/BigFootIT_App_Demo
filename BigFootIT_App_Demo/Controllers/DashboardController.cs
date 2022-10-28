using BigFootIT_App_Demo.Data;
using BigFootIT_App_Demo.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BigFootIT_App_Demo.Controllers
{
    
    public class DashboardController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _db;
        public DashboardController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }


        [Authorize]
        public async  Task<IActionResult> Customers()
        {
            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);

            if(loggedInUser == null)
            {
                TempData["error"] = "Your session has expired please sign in";
                return RedirectToAction("Index", "Home");
            }

            var userCustomers = _db.Customers.Where(x => x.IsActive == true && x.UserId.Equals(loggedInUser.Id)).ToList();
            return View(userCustomers);
        }


        [HttpPost]
        public async Task<IActionResult> Add_Customer(IFormCollection fc)
        {
            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);

            if (loggedInUser == null)
            {
                TempData["error"] = "Your session has expired please sign in";
                return RedirectToAction("Index", "Home");
            }

            CustomerEntity customer = new CustomerEntity
            {
                IsActive = true,
                UserId = loggedInUser.Id,
                CustomerAddress = fc["address"].ToString(),
                CustomerName = fc["name"]
            };

            await _db.Customers.AddAsync(customer);
            var status = await _db.SaveChangesAsync();

            if(status == 0)
            {
                TempData["error"] = "There was some technical error, user is not added. Please try again";
                return RedirectToAction("Customers", "Dashboard");
            }

            TempData["status"] = "Customer has been added successfully";
            return RedirectToAction("Customers","Dashboard");
        }

        public IActionResult View_Single_Customer(string customerId)
        {
            var singleCustomer = _db.Customers.Where(x => x.Id == Convert.ToInt32(customerId)).FirstOrDefault();
            return View(singleCustomer);
        }

        [HttpPost]
        public async Task<IActionResult> Update_Customer(IFormCollection fc)
        {
            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);

            if (loggedInUser == null)
            {
                TempData["error"] = "Your session has expired please sign in";
                return RedirectToAction("Index", "Home");
            }

            var singleCustomer = _db.Customers.Where(x => x.Id == Convert.ToInt32(fc["id"])).FirstOrDefault();
            if(singleCustomer ==null)
            {
                TempData["error"] = "There was some technical error, user is not updated. Please try again";
                return RedirectToAction("Customers", "Home");
            }

            singleCustomer.CustomerAddress = fc["address"].ToString();
            singleCustomer.CustomerName = fc["name"].ToString();

            var status = await _db.SaveChangesAsync();


            TempData["status"] = "Customer has been updated successfully";
            return RedirectToAction("View_Single_Customer", "Dashboard", new { customerId = fc["id"]});
        }


        [HttpPost]
        public async Task<IActionResult> Delete_Customer(IFormCollection fc)
        {
            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);

            if (loggedInUser == null)
            {
                TempData["error"] = "Your session has expired please sign in";
                return RedirectToAction("Index", "Home");
            }

            var singleCustomer = _db.Customers.Where(x => x.Id == Convert.ToInt32(fc["id"])).FirstOrDefault();
            if (singleCustomer == null)
            {
                TempData["error"] = "There was some technical error, user is not deleted. Please try again";
                return RedirectToAction("Customers", "Home");
            }

            singleCustomer.IsActive = false;

            var status = await _db.SaveChangesAsync();


            TempData["status"] = "Customer has been deleted successfully";
            return RedirectToAction("Customers", "Dashboard");
        }
    }
}
