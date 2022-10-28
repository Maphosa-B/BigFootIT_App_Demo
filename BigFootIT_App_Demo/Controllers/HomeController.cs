using BigFootIT_App_Demo.Data;
using BigFootIT_App_Demo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BigFootIT_App_Demo.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _db;
        public HomeController(UserManager<IdentityUser> userManager,SignInManager<IdentityUser> signInManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        } 
        /// <summary>
        /// This method will be used to display sign in viiew
        /// </summary>
        /// <returns> html view </returns>
        public IActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// This method is responsible for verifying user credintials for anuthetication and authorization
        /// If user is antheticated it will be redirected to dashboard home page else they will be prompted to re enter their credintials
        /// </summary>
        /// <param name="fc"> html form</param>
        /// <returns> view of sign in page or will be redirected to dashboard index</returns>
        [HttpPost]
        public async Task<IActionResult> Index(IFormCollection fc)
        {
            var email = fc["email"].ToString();
            var password = fc["password"].ToString();
            var strRemeberMe = fc["rememberMe"].ToString();
            bool blnRememberMe = false;

            //check if a user has checked remeber me
            if(strRemeberMe.Equals("On"))
            {
                blnRememberMe = true;
            }


            //checking if a user with the provided email exists
            var retrivedUser= await _userManager.FindByEmailAsync(email);
            
            //if a retrived user is null, then credintials are incorect
            if(retrivedUser == null)
            {
                TempData["error"] = "Incorect credintials";
                return RedirectToAction("Index","Home");
            }

            //check if a provided password matches with the user profile
            var signInStatus = await _signInManager.CheckPasswordSignInAsync(retrivedUser, password,false);

            //if credintials did not match then we redirect a user to try again
            if(signInStatus.Succeeded == false)
            {
                TempData["error"] = "Invalid credintials";
                return RedirectToAction("Index", "Home");
            }

            //signing in a user and redirect to dash board
            await _signInManager.SignInAsync(retrivedUser, blnRememberMe);
            return RedirectToAction("Customers", "Dashboard");
        }

        /// <summary>
        /// View for collecting user details for registration
        /// </summary>
        /// <returns>View</returns>
        public IActionResult Sign_Up()
        {
            return View();
        }

        /// <summary>
        /// method is responsible for collecting user data from view for registration
        /// </summary>
        /// <param name="fc"> form data</param>
        /// <returns> login page or register page if registration is not successful</returns>
        [HttpPost]
        public async Task<IActionResult> Sign_Up(IFormCollection fc)
        {

            var email = fc["email"].ToString();

            //checking if a user with the provided email exists
            var retrivedUser = await _userManager.FindByEmailAsync(email);

            //if a retrived user is not null, the provided email has alredy been used
            if (retrivedUser != null)
            {
                TempData["error"] = "A provided email address has alredy been used";
                return RedirectToAction("Sign_Up", "Home");
            }

            var checkCellPhoneNumber = _db.Users.Where(x => x.PhoneNumber.Equals((fc["phoneNumber"]))).FirstOrDefault();
            //if a retrived user is not null, the provided cell phone number has alredy been used
            if (checkCellPhoneNumber != null)
            {
                TempData["error"] = "A provided cell phone number has alredy been used";
                return RedirectToAction("Sign_Up", "Home");
            }

            //collecting a user data from view for user registration
            IdentityUser newUser = new IdentityUser
            {
                Email = fc["email"].ToString(),
                PhoneNumber = fc["phoneNumber"].ToString(),
                UserName = fc["email"].ToString(),
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
             var newUserStatus = await _userManager.CreateAsync(newUser);

            //check if a user is succefully registered
            if(newUserStatus.Succeeded == false)
            {
                TempData["error"] = "The was some technical errors, user is not registered. Please try again";
                return RedirectToAction("Sign_Up", "Home");
            }

            //add password to the newly registered user and check is successfully added
            var passwordStatus = await _userManager.AddPasswordAsync(newUser, fc["password"]);         
            if(passwordStatus.Succeeded == false)
            {
                await  _userManager.DeleteAsync(newUser);
                TempData["error"] = "The was some technical errors, user is not registered. Please try again";
                return RedirectToAction("Sign_Up", "Home");
            }

            TempData["status"] = "Account has been created, please sign in";
            return RedirectToAction("Index", "Home");
            
        }


    }
}