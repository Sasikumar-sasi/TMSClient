using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using TMSClient.Models;
using TMSClient.Models.ForeignKeyMapping;

namespace TMSClient.Controllers
{
    public class TrainersController : Controller
    {
        IConfiguration _configuration;
        string BaseURL;
        public TrainersController(IConfiguration configuration)
        {

            _configuration = configuration;
            BaseURL = _configuration.GetValue<string>("BaseURL");
        }
        public async Task<ActionResult> Index()
        {
            List<Trainer> trainers = await GetTrainers();
            return View(trainers);
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
        public async Task<ActionResult> Create(Trainer trainers)
        {
            try
            {
                Trainer recievedTrainer = new Trainer();
                HttpClientHandler clientHandler = new HttpClientHandler();
                var httpClient = new HttpClient(clientHandler);
                StringContent content = new StringContent(JsonConvert.SerializeObject(trainers), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync(BaseURL + "/api/Trainers", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    recievedTrainer = JsonConvert.DeserializeObject<Trainer>(apiResponse);
                    if (recievedTrainer != null)
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
            List<Trainer> hrs = await GetTrainers();
            Trainer tr = hrs.FirstOrDefault(a => a.TrainerID == id);
            return View(tr);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Trainer tr)
        {
            try
            {
                tr.TrainerID = id;
                HttpClientHandler clientHandler = new HttpClientHandler();
                var httpClient = new HttpClient(clientHandler);
                StringContent contents = new StringContent(JsonConvert.SerializeObject(tr), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PutAsync(BaseURL + "/api/trainers/" + id, contents))
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
        public async Task<IActionResult> Login(Admin admin)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Inputs are not valid";
                return View();
            }
            List<Trainer> trainers = await GetTrainers();
            var obj = trainers.Where(a => a.EmailID.Equals(admin.EmailID) && a.Password.Equals(admin.Password)).FirstOrDefault();
            if (obj != null)
            {
                HttpContext.Session.SetString("ID", obj.TrainerID.ToString());
                HttpContext.Session.SetString("EmailID", obj.EmailID.ToString());
                HttpContext.Session.SetString("Role", obj.Role.ToString());
                return RedirectToAction("DashBoard", "trainers");
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

        public async Task<IActionResult> ViewAssessmentWithName()
        {
            List<Batch> batches = await GetAllBatchs();
            int id = Convert.ToInt32(HttpContext.Session.GetString("ID"));
            Console.WriteLine(id);
            Batch batch = batches.FirstOrDefault(b => b.TrainerID == id);
            Console.WriteLine(batch.BatchID);
            List<Assessment> assessments = await GetAllAssessments();
            List<Assessment> yourAssessment = assessments.Where(ass=>ass.BatchID==batch.BatchID).ToList();
            List<AssessmentBatch> assessmentBatches = new List<AssessmentBatch>();
            foreach (var item in yourAssessment)
            {

                AssessmentBatch assessmentBatch = new AssessmentBatch()
                {
                    AssessmentID = item.AssessmentID,
                    AssessmentName = item.AssessmentName,
                    BatchName = batch.BatchName,
                    Date = item.Date,
                    Duration = item.Duration,
                    EndingTime = item.EndingTime,
                    Question = item.Question,
                    StartingTime = item.StartingTime
                };
                assessmentBatches.Add(assessmentBatch);
            }
            return View(assessmentBatches);
        }


        public async Task<IActionResult> ViewAssessmentScores(int id)
        {
            if (HttpContext.Session.GetString("Role").ToString().Equals("Trainer"))
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


        public async Task<List<Trainer>> GetTrainers()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            HttpClient client = new HttpClient(clientHandler);
            string JsonStr = await client.GetStringAsync(BaseURL + "/api/trainers");
            var result = JsonConvert.DeserializeObject<List<Trainer>>(JsonStr);
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

        public async Task<List<Trainee>> GetTrainees()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            HttpClient client = new HttpClient(clientHandler);
            string JsonStr = await client.GetStringAsync(BaseURL + "/api/trainees");
            var result = JsonConvert.DeserializeObject<List<Trainee>>(JsonStr);
            return result;
        }

        public async Task<IActionResult> ViewYourTrainees()
        {
            List<Batch> batches = await GetAllBatchs();
            int id =Convert.ToInt32(HttpContext.Session.GetString("ID"));
            Batch batch =  batches.FirstOrDefault(b=>b.TrainerID==id);
            List<Trainee> trainees = await GetTrainees();
            List<Trainee> yourTrainees = trainees.Where(tr => tr.BatchID == batch.BatchID).ToList();
            return View(yourTrainees);
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
