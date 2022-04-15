using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using TMSClient.Models;

namespace TMSClient.Controllers
{
    public class TraineesController : Controller
    {
        IConfiguration _configuration;
        string BaseURL;
        public TraineesController(IConfiguration configuration)
        {

            _configuration = configuration;
            BaseURL = _configuration.GetValue<string>("BaseURL");
        }


        // GET: TraineesController
        public async Task<ActionResult> Index()
        {
            List<Trainee> trainees = await GetTrainees();
            return View(trainees);
        }
        // GET: TraineesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TraineesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TraineesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Trainee trainee)
        {
            try
            {
                Trainee recievedTrainees = new Trainee();
                HttpClientHandler clientHandler = new HttpClientHandler();
                var httpClient = new HttpClient(clientHandler);
                StringContent content = new StringContent(JsonConvert.SerializeObject(trainee), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync(BaseURL + "/api/trainees", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    recievedTrainees = JsonConvert.DeserializeObject<Trainee>(apiResponse);

                    if (recievedTrainees != null)
                    {
                        return RedirectToAction("DashBoard", "hrs");
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

        // GET: TraineesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TraineesController/Edit/5
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

        // GET: TraineesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TraineesController/Delete/5
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

        public async Task<List<Trainee>> GetTrainees()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            HttpClient client = new HttpClient(clientHandler);
            string JsonStr = await client.GetStringAsync(BaseURL + "/api/trainees");
            var result = JsonConvert.DeserializeObject<List<Trainee>>(JsonStr);
            return result;
        }
    }
}
