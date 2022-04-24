using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using TMSClient.AppLogger;
using TMSClient.Models;
using TMSClient.Models.ForeignKeyMapping;

namespace TMSClient.Controllers
{
    public class HRsController : Controller
    {
        IConfiguration _configuration;
        string BaseURL;
        private readonly ILoggerManager logger;
        public HRsController(IConfiguration configuration,ILoggerManager logger)
        {
            this.logger = logger;
            this.logger.LogInformation("On HR Controller constructor initialized");
            _configuration = configuration;
            BaseURL = _configuration.GetValue<string>("BaseURL");
        }


        public ActionResult Create()
        {
            if (HttpContext.Session.GetString("Role").ToString().Equals("ADMIN"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login","Admins");
            }
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

        public async Task<ActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("Role").ToString().Equals("HR"))
            {
                List<HR> hrs = await GetHRs();
                HR hr = hrs.FirstOrDefault(a => a.HRId == id);
                return View(hr);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, HR hr)
        {
            try
            {
                hr.HRId = id;
                HttpClientHandler clientHandler = new HttpClientHandler();
                var httpClient = new HttpClient(clientHandler);
                StringContent contents = new StringContent(JsonConvert.SerializeObject(hr), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PutAsync(BaseURL + "/api/hrs/" + id, contents))
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
            List<HR> hrs = await GetHRs();
            var obj = hrs.Where(a => a.EmailID.Equals(hr.EmailID) && a.Password.Equals(hr.Password)).FirstOrDefault();
            if (obj != null)
            {
                this.logger.LogInformation($"{obj.Name} - Logged in");
                HttpContext.Session.SetString("EmailID", obj.EmailID.ToString());
                HttpContext.Session.SetString("ID", obj.HRId.ToString());
                HttpContext.Session.SetString("Role", obj.Role.ToString());
                return RedirectToAction("DashBoard", "Hrs");
            }
            else
            {
                this.logger.LogInformation("HR credentials id wrong");
                ViewBag.Message = "User not found for given Email and Password";
                return View();
            }
        }

        public IActionResult DashBoard()
        {

            if (HttpContext.Session.GetString("Role").ToString().Equals("HR")) 
            {
                this.logger.LogInformation($"{HttpContext.Session.GetString("EmailID")} - DashBoard");
                return View();
            }
            else
            {
                return RedirectToAction("Login");            
            }
        }

        public async Task<IActionResult> AllTrainerManager()
        {
            if (HttpContext.Session.GetString("Role").ToString().Equals("HR"))
            {
                List<TrainerManager> trainerManagers = await GetTrainerManagers();
                return View(trainerManagers);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public async Task<IActionResult> AllTrainers()
        {
            if (HttpContext.Session.GetString("Role").ToString().Equals("HR"))
            {
                List<Trainer> trainerManagers = await GetTrainers();
                return View(trainerManagers);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public async Task<IActionResult> AllTrainees()
        {
            if (HttpContext.Session.GetString("Role").ToString().Equals("HR"))
            {
                List<Trainee> trainerManagers = await GetTrainees();
                return View(trainerManagers);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public async Task<ActionResult> ViewAllAssessment()
        {
            if (HttpContext.Session.GetString("Role").Equals("HR"))
            {
                List<Assessment> assessments = await GetAllAssessments();

                return View(assessments);
            }
            else
            {
                return RedirectToAction("Login", "HRs");
            }
        }


        public async Task<IActionResult> ViewAssessmentScores(int id)
        {
            if (HttpContext.Session.GetString("Role").ToString().Equals("HR"))
            {
                List<Score> scores = await GetAllScores();
                List<Assessment> assessments = await GetAllAssessments();
                List<Trainee> trainees = await GetTrainees();
                List<Score> scoresBasedID = scores.Where(s => s.AssessmentID == id).ToList();

                List<ScoreWithName> assessmentName = new List<ScoreWithName>();


                foreach (var item in scoresBasedID)
                {
                    Assessment assessment = assessments.FirstOrDefault(ass => ass.AssessmentID == item.AssessmentID);
                    Trainee trainee = trainees.FirstOrDefault(tr => tr.TraineeID == item.TraineeID);
                    ScoreWithName scoreWithName = new ScoreWithName()
                    {
                        ScoreID = item.ScoreID,
                        AssessmentName = assessment.AssessmentName,
                        GainedScore = item.GainedScore,
                        TotalScore = item.TotalScore,
                        TraineeName = trainee.Name
                    };
                    assessmentName.Add(scoreWithName);
                }
                return View(assessmentName);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public async Task<List<Score>> GetAllScores()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            HttpClient client = new HttpClient(clientHandler);
            string JsonStr = await client.GetStringAsync(BaseURL + "/api/scores");
            var result = JsonConvert.DeserializeObject<List<Score>>(JsonStr);
            return result;
        }
        public async Task<List<Assessment>> GetAllAssessments()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            HttpClient client = new HttpClient(clientHandler);
            string JsonStr = await client.GetStringAsync(BaseURL + "/api/assessments");
            var result = JsonConvert.DeserializeObject<List<Assessment>>(JsonStr);
            return result;
        }

        public async Task<List<HR>> GetHRs()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            HttpClient client = new HttpClient(clientHandler);
            string JsonStr = await client.GetStringAsync(BaseURL + "/api/hrs");
            var result = JsonConvert.DeserializeObject<List<HR>>(JsonStr);
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
