using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using TMSClient.Models;
using TMSClient.Models.ForeignKeyMapping;

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
        
        public async Task<ActionResult> Create()
        {
            if (HttpContext.Session.GetString("Role").ToString().Equals("HR"))
            {
                var batches = await GetAllBatchs();
                ViewData["BatchName"] = new SelectList(batches, "BatchID", "BatchName");
                return View();
            }
            else
            {
                return RedirectToAction("Login", "HRs");
            }
        }

        // POST: TraineesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Trainee trainee)
        {
            try
            {
                //List<Batch> batches = await GetAllBatchs();
                //Batch batch = batches.FirstOrDefault(b=>b.BatchID==trainee.BatchID);
                //Console.WriteLine(batch.BatchName);
                //trainee.Batchs = batch;
                //Console.WriteLine(trainee.Batchs.BatchName);
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

        public async Task<ActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("Role").ToString().Equals("Trainee"))
            {
                List<Trainee> tms = await GetTrainees();
                Trainee tm = tms.FirstOrDefault(a => a.TraineeID == id);
                return View(tm);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Trainee tr)
        {
            try
            {
                tr.TraineeID = id;
                HttpClientHandler clientHandler = new HttpClientHandler();
                var httpClient = new HttpClient(clientHandler);
                StringContent contents = new StringContent(JsonConvert.SerializeObject(tr), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PutAsync(BaseURL + "/api/trainees/" + id, contents))
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
            if (HttpContext.Session.GetString("Role").ToString().Equals("Trainee"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }




        public async Task<IActionResult> ViewAssessment()
        {
            int BatchID = Convert.ToInt32(HttpContext.Session.GetString("Batch"));
            List<Assessment> assessments = await GetAllAssessments();
            List<Assessment> traineeAssessment = assessments.Where(ass => ass.BatchID == BatchID).ToList();
            return View(traineeAssessment);
        }


        public async Task<IActionResult> ViewAssessmentScores()
        {
            List<Score> scores = await GetAllScores();
            List<Assessment> assessments = await GetAllAssessments();
            int id = Convert.ToInt32(HttpContext.Session.GetString("ID"));
            List<Score> scoresBasedID = scores.Where(s => s.TraineeID == id).ToList();

            List<ScoreWithName> assessmentName = new List<ScoreWithName>();
            

            foreach (var item in scoresBasedID)
            {
                Assessment assessment = assessments.FirstOrDefault(ass=>ass.AssessmentID==item.AssessmentID);
                ScoreWithName scoreWithName = new ScoreWithName()
                {
                    ScoreID = item.ScoreID,
                    AssessmentName = assessment.AssessmentName,
                    GainedScore = item.GainedScore,
                    TotalScore = item.TotalScore,
                };
                assessmentName.Add(scoreWithName);
            }
            return View(assessmentName);
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
