using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using TMSClient.Models;
using TMSClient.Models.ViewModel;

namespace TMSClient.Controllers
{
    public class AssessmentsController : Controller
    {
        IConfiguration _configuration;
        string BaseURL;
        IHostEnvironment _environment;



        public AssessmentsController(IConfiguration configuration, IHostEnvironment environment)
        {
            _environment = environment;
            _configuration = configuration;
            BaseURL = _configuration.GetValue<string>("BaseURL");
        }
        public async Task<ActionResult> Index()
        {
            if (HttpContext.Session.GetString("Role").Equals("Training Manager"))
            {
                List<Assessment> assessments = await GetAllAssessments();

                return View(assessments);
            }
            else
            {
                return RedirectToAction("Login","TrainerManagers");
            }
        }

        public async Task<ActionResult> Create()
        {
            if (HttpContext.Session.GetString("Role").Equals("Training Manager"))
            {
                var batches = await GetAllBatchs();
                ViewData["BatchName"] = new SelectList(batches, "BatchID", "BatchName");
                return View();
            }
            else
            {
                return RedirectToAction("Login", "TrainerManagers");
            }
        }

        // POST: AssessmentsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Assessment assessment)
        {
            try
            {
                //List<Batch> batches = await GetAllBatchs();
                //Batch batch = batches.FirstOrDefault(b => b.BatchID == assessment.BatchID);
                //assessment.Batchs = batch;
                //Console.WriteLine(assessment.Batchs.BatchName);
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

        public async Task<ActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("Role").Equals("Trainer"))
            {
                List<Assessment> assessments = await GetAllAssessments();
                Assessment assessment = assessments.FirstOrDefault(ass => ass.AssessmentID == id);
                AssessmentViewModel assessmentViewModel = new AssessmentViewModel()
                {
                    AssessmentName = assessment.AssessmentName,
                    BatchID = assessment.BatchID,
                    Date = assessment.Date,
                    Duration = assessment.Duration,
                    EndingTime = assessment.EndingTime,
                    StartingTime = assessment.StartingTime,
                    Batchs = assessment.Batchs,
                };
                return View(assessmentViewModel);
            }
            else
            {
                return RedirectToAction("Login", "Trainers");
            }
        }

        // POST: AssessmentsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, AssessmentViewModel assessmentViewModel)
        {
            try
            {
                Console.WriteLine(id+assessmentViewModel.AssessmentName);
                Console.WriteLine(assessmentViewModel.QuestionFile.FileName);
                string UniqueFileName = null;
                if (assessmentViewModel.QuestionFile != null)
                {
                    Console.WriteLine("Checking folder");
                    string UploadFolder = "QuestionsAndAnswers/Questions";
                    Console.WriteLine("Folder is correct");


                    UniqueFileName = Guid.NewGuid().ToString() + "-" + assessmentViewModel.QuestionFile.FileName;
                    string FilePath = Path.Combine(UploadFolder, UniqueFileName + Path.GetExtension(assessmentViewModel.QuestionFile.FileName));
                    assessmentViewModel.QuestionFile.CopyTo(new FileStream(FilePath, FileMode.Create));

                    Assessment assessment = new Assessment()
                    {
                        AssessmentID = id,
                        AssessmentName = assessmentViewModel.AssessmentName,
                        Date = assessmentViewModel.Date,
                        Duration = assessmentViewModel.Duration,
                        EndingTime = assessmentViewModel.EndingTime,
                        StartingTime = assessmentViewModel.StartingTime,
                        BatchID = assessmentViewModel.BatchID,
                        Batchs = assessmentViewModel.Batchs,
                        Question = UniqueFileName

                    };

                    HttpClientHandler clientHandler = new HttpClientHandler();

                    var httpClient = new HttpClient(clientHandler);
                    StringContent contents = new StringContent(JsonConvert.SerializeObject(assessment), Encoding.UTF8, "application/json");

                    var response = await httpClient.PutAsync(BaseURL + "/api/Assessments/" + id, contents);
                    string apiResponse = await response.Content.ReadAsStringAsync(); ;
                    if (apiResponse != null)
                        return RedirectToAction("DashBoard", "Trainers");
                    else
                    {
                        Console.WriteLine("Cannot saved to Server");
                        return View();
                    }
                }
                else
                {
                    Console.WriteLine("FileName is Empty");
                    return View();
                }

            }
            catch
            {
                Console.WriteLine("Catch Bloack executed");
                return View();
            }
        }

        public async Task<IActionResult> Download(string fileName)
        {

            if (fileName == null)
                return Content("filename not present");
            string QuestionFolder = "QuestionsAndAnswers/Questions";

            var path = Path.Combine(QuestionFolder, fileName + Path.GetExtension(fileName));

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            var contentType = "APPLICATION/octet-stream";
            return File(memory, contentType, Path.GetFileName(path));
        }


        public IActionResult UploadScore(int id, int traineeID)
        {
            if (HttpContext.Session.GetString("Role").Equals("Trainer"))
            {
                ViewBag.AssessmentIDForScore = id;
                ViewBag.TraineeIDForScore = traineeID;
                return View();

            }
            else
            {
                return RedirectToAction("Login", "Trainers");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UploadScore(Score score)
        {
            try
            {
                Score recievedScore = new Score();
                HttpClientHandler clientHandler = new HttpClientHandler();
                var httpClient = new HttpClient(clientHandler);
                StringContent content = new StringContent(JsonConvert.SerializeObject(score), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync(BaseURL + "/api/Scores", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    recievedScore = JsonConvert.DeserializeObject<Score>(apiResponse);
                    if (recievedScore != null)
                    {
                        return RedirectToAction("Index", "Answers",new { id = score.AssessmentID});
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
