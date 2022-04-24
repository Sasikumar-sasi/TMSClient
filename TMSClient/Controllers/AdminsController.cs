using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using TMSClient.AppLogger;
using TMSClient.Models;


namespace TMSClient.Controllers
{
    public class AdminsController : Controller
    {
        
        
        IConfiguration _configuration;
        string BaseURL;
        private readonly ILoggerManager logger;
        public AdminsController(IConfiguration configuration, ILoggerManager logger)
        {
            this.logger = logger;
            this.logger.LogInformation("On the Admin Controller constructor");

            _configuration = configuration;
            BaseURL = _configuration.GetValue<string>("BaseURL");
        }

        public async Task<ActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("Role").Equals("ADMIN")) 
            {
                List<Admin> admins = await GetAdmin();
                Admin admin = admins.FirstOrDefault(a => a.AdminId == id);
                return View(admin);
            }
            else
            {
                return RedirectToAction("Login");
            }
            
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Admin admin)
        {
            try
            {
               admin.AdminId= id;
                HttpClientHandler clientHandler = new HttpClientHandler();
                var httpClient = new HttpClient(clientHandler);
                StringContent contents = new StringContent(JsonConvert.SerializeObject(admin), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PutAsync(BaseURL + "/api/Admins/" + id, contents))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    if (apiResponse != null)
                        return RedirectToAction("DashBoard");
                    else
                        return View();
                }
            }
            catch
            {
                return View();
            }
        }

       

        [HttpGet]
        public IActionResult Login()
        {
            this.logger.LogInformation("Admin Login Page Loaded");

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
                this.logger.LogInformation($"{obj.AdminName} - Logined  Successful");
                HttpContext.Session.SetString("ID", obj.AdminId.ToString());
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
            if (HttpContext.Session.GetString("Role").Equals("ADMIN"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public async Task<IActionResult> AllHRs()
        {
            if (HttpContext.Session.GetString("Role").Equals("ADMIN"))
            {
                this.logger.LogInformation("Admin - View all HR Information");
                List<HR> hrs = await GetHRs();
                return View(hrs);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public async Task<IActionResult> AllTrainerManager()
        {
            if (HttpContext.Session.GetString("Role").Equals("ADMIN"))
            {
                this.logger.LogInformation("Admin - View all Trainer Manager Information");
                List<TrainerManager> tms = await GetTrainerManagers();
                return View(tms);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public async Task<IActionResult> AllTrainer()
        {
            if (HttpContext.Session.GetString("Role").Equals("ADMIN"))
            {
                this.logger.LogInformation("Admin - View all Trainer Information");
                List<Trainer> tr = await GetTrainers();
                return View(tr);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public async Task<IActionResult> AllTrainees()
        {
            if (HttpContext.Session.GetString("Role").Equals("ADMIN"))
            {
                this.logger.LogInformation("Admin - View all Trainer's Information");
                List<Trainee> tr = await GetTrainees();
                return View(tr);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public async Task<List<HR>> GetHRs()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            HttpClient client = new HttpClient(clientHandler);
            string JsonStr = await client.GetStringAsync(BaseURL + "/api/hrs");
            var result = JsonConvert.DeserializeObject<List<HR>>(JsonStr);
            return result;
        }

        public async Task<List<Admin>> GetAdmin()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            HttpClient client = new HttpClient(clientHandler);
            string JsonStr = await client.GetStringAsync(BaseURL + "/api/admins");
            var result = JsonConvert.DeserializeObject<List<Admin>>(JsonStr);
            return result;
        }
        public async Task<List<Trainee>> GetTrainees()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            HttpClient client = new HttpClient(clientHandler);
            string JsonStr = await client.GetStringAsync(BaseURL + "/api/trainees");
            var result = JsonConvert.DeserializeObject<List<Trainee>>(JsonStr);
            return result;
        }
        public async Task<List<TrainerManager>> GetTrainerManagers()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            HttpClient client = new HttpClient(clientHandler);
            string JsonStr = await client.GetStringAsync(BaseURL + "/api/trainermanagers");
            var result = JsonConvert.DeserializeObject<List<TrainerManager>>(JsonStr);
            return result;
        }
        public async Task<List<Trainer>> GetTrainers()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            HttpClient client = new HttpClient(clientHandler);
            string JsonStr = await client.GetStringAsync(BaseURL + "/api/trainers");
            var result = JsonConvert.DeserializeObject<List<Trainer>>(JsonStr);
            return result;
        }
    }
}
