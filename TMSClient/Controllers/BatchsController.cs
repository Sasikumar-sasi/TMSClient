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
            List<Batch> batchList = await GetAllBatchs();
            return View(batchList);
        }

        // GET: BatchsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BatchsController/Create
        public async Task<ActionResult> Create()
        {
            var trainers = await GetTrainers();
            ViewData["TrainerName"] = new SelectList(trainers, "TrainerID", "Name");
            return View();
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
        public ActionResult Edit(int id)
        {
            return View();
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

        // GET: BatchsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BatchsController/Delete/5
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
