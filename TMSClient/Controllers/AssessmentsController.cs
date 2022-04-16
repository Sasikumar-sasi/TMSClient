using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using TMSClient.Models;

namespace TMSClient.Controllers
{
    public class AssessmentsController : Controller
    {
        IConfiguration _configuration;
        string BaseURL;
        public AssessmentsController(IConfiguration configuration)
        {

            _configuration = configuration;
            BaseURL = _configuration.GetValue<string>("BaseURL");
        }
        public async Task<ActionResult> Index()
        {
            List<Assessment> assessments = await GetAllAssessments();

            return View(assessments);
        }

        // GET: AssessmentsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AssessmentsController/Create
        public async Task<ActionResult> Create()
        {
            var batches = await GetAllBatchs();
            ViewData["BatchName"] = new SelectList(batches, "BatchID", "BatchName");
            return View();
        }

        // POST: AssessmentsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Assessment assessment)
        {
            try
            {
                Assessment recievedAssessment = new Assessment();
                HttpClientHandler clientHandler = new HttpClientHandler();
                var httpClient = new HttpClient(clientHandler);
                StringContent content = new StringContent(JsonConvert.SerializeObject(assessment), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync(BaseURL + "/api/Assessments", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    recievedAssessment = JsonConvert.DeserializeObject<Assessment>(apiResponse);
                    if (recievedAssessment != null)
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

        // GET: AssessmentsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AssessmentsController/Edit/5
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

        // GET: AssessmentsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AssessmentsController/Delete/5
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

        public async Task<List<Assessment>> GetAllAssessments()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            HttpClient client = new HttpClient(clientHandler);
            string JsonStr = await client.GetStringAsync(BaseURL + "/api/assessments");
            var result = JsonConvert.DeserializeObject<List<Assessment>>(JsonStr);
            return result;
        }
        public async Task<List<Batch>> GetAllBatchs()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            HttpClient client = new HttpClient(clientHandler);
            string JsonStr = await client.GetStringAsync(BaseURL + "/api/Batches");
            var result = JsonConvert.DeserializeObject<List<Batch>>(JsonStr);
            return result;
        }
    }
}
