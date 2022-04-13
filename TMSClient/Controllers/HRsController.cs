using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using TMSClient.Models;

namespace TMSClient.Controllers
{
    public class HRsController : Controller
    {
        IConfiguration _configuration;
        string BaseURL;
        public HRsController(IConfiguration configuration)
        {

            _configuration = configuration;
            BaseURL = _configuration.GetValue<string>("BaseURL");
        }

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(HR hr)
        {
            try
            {

                HR recievedHR = new HR();
                HttpClientHandler clientHandler = new HttpClientHandler();
                var httpClient = new HttpClient(clientHandler);
                StringContent content = new StringContent(JsonConvert.SerializeObject(hr), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync(BaseURL + "/api/hrs", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    recievedHR = JsonConvert.DeserializeObject<HR>(apiResponse);
                    if (recievedHR != null)
                    {
                        return RedirectToAction("DashBoard","Admins");
                    }
                }


                ViewBag.Message = "Your Record not Created!!! Please try again";
                return View();

            }
            catch
            {
                return View();
            }
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

        public ActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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
        public async Task<IActionResult> Login(HR hr)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Inputs are not valid";
                return View();
            }
            List<Admin> admins = await GetAdmin();
            var obj = admins.Where(a => a.EmailID.Equals(hr.EmailID) && a.Password.Equals(hr.Password)).FirstOrDefault();
            if (obj != null)
            {
                HttpContext.Session.SetString("EmailID", obj.EmailID.ToString());
                HttpContext.Session.SetString("Role", obj.Role.ToString());
                return RedirectToAction("DashBoard", "Hrs");
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
