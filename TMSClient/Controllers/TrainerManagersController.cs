using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using TMSClient.Models;
using TMSClient.Models.ForeignKeyMapping;

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
        public async Task<ActionResult> Index()
        {
            if (HttpContext.Session.GetString("Role").ToString().Equals("HR"))
            {
                List<TrainerManager> trainerManagers = await GetTrainerManagers();
                return View(trainerManagers);
            }
            else
            {
                return RedirectToAction("Login", "Hrs");
            }
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Create()
        {
            if (HttpContext.Session.GetString("Role").ToString().Equals("HR"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "HRs");
            }
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
                        return RedirectToAction("DashBoard", "Hrs");
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
            if (HttpContext.Session.GetString("Role").ToString().Equals("Training Manager"))
            {
                List<TrainerManager> tms = await GetTrainerManagers();
                TrainerManager tm = tms.FirstOrDefault(a => a.TMID == id);
                return View(tm);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, TrainerManager tr)
        {
            try
            {
                tr.TMID = id;
                HttpClientHandler clientHandler = new HttpClientHandler();
                var httpClient = new HttpClient(clientHandler);
                StringContent contents = new StringContent(JsonConvert.SerializeObject(tr), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PutAsync(BaseURL + "/api/trainermanagers/" + id, contents))
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

        public async Task<IActionResult> ViewAssessmentScores(int id)
        {
            if (HttpContext.Session.GetString("Role").ToString().Equals("Training Manager"))
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
        public async Task<IActionResult> Login(TrainerManager tm)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Inputs are not valid";
                return View();
            }
            List<TrainerManager> admins = await GetTrainerManagers();
            var obj = admins.Where(a => a.EmailID.Equals(tm.EmailID) && a.Password.Equals(tm.Password)).FirstOrDefault();
            if (obj != null)
            {

                HttpContext.Session.SetString("EmailID", obj.EmailID.ToString());
                HttpContext.Session.SetString("ID", obj.TMID.ToString());
                HttpContext.Session.SetString("Role", obj.Role.ToString());
                return RedirectToAction("DashBoard", "TrainerManagers");
            }
            else
            {
                ViewBag.Message = "User not found for given Email and Password";
                return View();
            }
        }


        public IActionResult DashBoard()
        {
            if (HttpContext.Session.GetString("Role").ToString().Equals("Training Manager"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        public async Task<IActionResult> AllTrainers()
        {
            List<Trainer> trainerManagers = await GetTrainers();
            return View(trainerManagers);
        }

        public async Task<IActionResult> AllTrainees()
        {
            List<Trainee> trainerManagers = await GetTrainees();
            return View(trainerManagers);
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
    }
}
