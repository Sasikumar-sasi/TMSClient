using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TMSClient.Models;

namespace TMSClient.Controllers
{
    public class AdminsController : Controller
    {
        
        
        IConfiguration _configuration;
        string BaseURL;
        public AdminsController(IConfiguration configuration)
        {
            
            _configuration = configuration;
            BaseURL = _configuration.GetValue<string>("BaseURL");
        }


        public ActionResult Edit(int id)
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

       

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Admin admin)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Inputs are not valid";
                return View();
            }
            List<Admin> admins = await GetAdmin();
            var obj = admins.Where(a => a.EmailID.Equals(admin.EmailID) && a.Password.Equals(admin.Password)).FirstOrDefault();
            if (obj != null)
            {
                HttpContext.Session.SetString("EmailID", obj.EmailID.ToString());
                HttpContext.Session.SetString("Role", obj.Role.ToString());
                return RedirectToAction("DashBoard", "Admins");
            }
            else
            {
                ViewBag.Message = "User not found for given Email and Password";
                return View();
            }
        }

        public IActionResult DashBoard()
        {
            return View();
        }


        [HttpGet]
        public async Task<List<Admin>> GetAdmin()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            HttpClient client = new HttpClient(clientHandler);
            string JsonStr = await client.GetStringAsync(BaseURL + "/api/admins");
            var result = JsonConvert.DeserializeObject<List<Admin>>(JsonStr);
            return result;
        }
    }
}
