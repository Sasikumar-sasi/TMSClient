using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public async Task<ActionResult> Create()
        {
            var batches = await GetAllBatchs();
            ViewData["BatchName"] = new SelectList(batches, "BatchID", "BatchName");
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


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Trainee trainee)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Inputs are not valid";
                return View();
            }
            List<Trainee> trainees = await GetTrainees();
            var obj = trainees.Where(a => a.EmailID.Equals(trainee.EmailID) && a.Password.Equals(trainee.Password)).FirstOrDefault();
            if (obj != null)
            {
                HttpContext.Session.SetString("ID", obj.TraineeID.ToString());
                HttpContext.Session.SetString("EmailID", obj.EmailID.ToString());
                HttpContext.Session.SetString("Batch", obj.BatchID.ToString());
                HttpContext.Session.SetString("Role", obj.Role.ToString());
                return RedirectToAction("DashBoard", "Trainees");
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




        public async Task<IActionResult> ViewAssessment()
        {
            int BatchID = Convert.ToInt32(HttpContext.Session.GetString("Batch"));
            List<Assessment> assessments = await GetAllAssessments();
            List<Assessment> traineeAssessment = assessments.Where(ass => ass.BatchID == BatchID).ToList();
            return View(traineeAssessment);
        }


        public async Task<IActionResult> ViewScores()
        {
            List<Score> scores = await GetAllScores();
            List<Assessment> assessments = await GetAllAssessments();
            int id = Convert.ToInt32(HttpContext.Session.GetString("ID"));
            List<Score> scoresBasedID = scores.Where(s => s.TraineeID == id).ToList();
            List<Assessment> assessmentName = new List<Assessment>();
            foreach(Score score in scoresBasedID)
            {
               
                Assessment assessment = assessments.FirstOrDefault(ass => ass.AssessmentID==score.AssessmentID);
                assessmentName.Add(assessment);
            }
            ViewData["AssessmentName"] = assessmentName;
            
            return View(scoresBasedID);
        }


        public async Task<List<Assessment>> GetAllAssessments()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            HttpClient client = new HttpClient(clientHandler);
            string JsonStr = await client.GetStringAsync(BaseURL + "/api/assessments");
            var result = JsonConvert.DeserializeObject<List<Assessment>>(JsonStr);
            return result;
        }

        public async Task<List<Score>> GetAllScores()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            HttpClient client = new HttpClient(clientHandler);
            string JsonStr = await client.GetStringAsync(BaseURL + "/api/scores");
            var result = JsonConvert.DeserializeObject<List<Score>>(JsonStr);
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
