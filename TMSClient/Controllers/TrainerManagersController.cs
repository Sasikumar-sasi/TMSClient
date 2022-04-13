using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using TMSClient.Models;

namespace TMSClient.Controllers
{
    public class TrainerManagersController : Controller
    {
        IConfiguration _configuration;
        string BaseURL;
        public TrainerManagersController(IConfiguration configuration)
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
        public async Task<ActionResult> Create(TrainerManager trainerManager)
        {
            try
            {

                TrainerManager recievedTM = new TrainerManager();
                HttpClientHandler clientHandler = new HttpClientHandler();
                var httpClient = new HttpClient(clientHandler);
                StringContent content = new StringContent(JsonConvert.SerializeObject(trainerManager), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync(BaseURL + "/api/TrainerManagers", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    recievedTM = JsonConvert.DeserializeObject<TrainerManager>(apiResponse);
                    if (recievedTM != null)
                    {
                        return RedirectToAction("DashBoard", "trainermanagers");
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
    }
}
