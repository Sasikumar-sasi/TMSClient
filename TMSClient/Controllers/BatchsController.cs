using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using TMSClient.Models;

namespace TMSClient.Controllers
{
    public class BatchsController : Controller
    {
        IConfiguration _configuration;
        string BaseURL;
        public BatchsController(IConfiguration configuration)
        {

            _configuration = configuration;
            BaseURL = _configuration.GetValue<string>("BaseURL");
        }
        public async Task<ActionResult> Index()
        {
            if (HttpContext.Session.GetString("Role").Equals("Training Manager"))
            {
                List<Batch> batchList = await GetAllBatchs();
                return View(batchList);
            }
            else
            {
                return RedirectToAction("Login", "TrainerManagers");
            }
        }

        public async Task<ActionResult> Create()
        {
            if (HttpContext.Session.GetString("Role").ToString().Equals("Training Manager"))
            {
                var trainers = await GetTrainers();
                ViewData["TrainerName"] = new SelectList(trainers, "TrainerID", "Name");
                return View();
            }
            else
            {
                return RedirectToAction("Login", "TrainerManaers");
            }
        }

        // POST: BatchsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Batch batch)
        {
            try
            {
                Batch recievedBatch = new Batch();
                HttpClientHandler clientHandler = new HttpClientHandler();
                var httpClient = new HttpClient(clientHandler);
                StringContent content = new StringContent(JsonConvert.SerializeObject(batch), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync(BaseURL + "/api/Batches", content))
                {
                    Console.WriteLine("Inside use");
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("wait for api result");
                    recievedBatch = JsonConvert.DeserializeObject<Batch>(apiResponse);
                    Console.WriteLine("recived ");
                    if (recievedBatch != null)
                    {
                        Console.WriteLine("Inside if");
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

        // GET: BatchsController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("Role").ToString().Equals("Training Manager"))
            {
                var trainers = await GetTrainers();
                ViewData["TrainerName"] = new SelectList(trainers, "TrainerID", "Name");
                List<Batch> batches = await GetAllBatchs();
                Batch batch = batches.FirstOrDefault(a => a.BatchID == id);
                return View(batch);
            }
            else
            {
                return RedirectToAction("Login", "TrainerManagers");
            }
        }

        // POST: BatchsController/Edit/5
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

        public async Task<List<Batch>> GetAllBatchs()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            HttpClient client = new HttpClient(clientHandler);
            string JsonStr = await client.GetStringAsync(BaseURL + "/api/Batches");
            var result = JsonConvert.DeserializeObject<List<Batch>>(JsonStr);
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
