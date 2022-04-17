using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
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


        public async Task<ActionResult> Edit(int id)
        {
            List<Admin> admins = await GetAdmin() ;
            Admin admin = admins.FirstOrDefault(a=>a.AdminId==id);
            return View(admin);
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
            return View();
        }

        public async Task<IActionResult> AllHRs()
        {
            List<HR> hrs= await GetHRs();
            return View(hrs);
        }

        public async Task<IActionResult> AllTrainerManager()
        {
            List<TrainerManager> tms = await GetTrainerManagers();
            return View(tms);
        }
        public async Task<IActionResult> AllTrainer()
        {
            List<Trainer> tr = await GetTrainers();
            return View(tr);
        }
        public async Task<IActionResult> AllTrainees()
        {
            List<Trainee> tr = await GetTrainees();
            return View(tr);
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
