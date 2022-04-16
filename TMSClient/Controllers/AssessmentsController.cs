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
        //[HttpGet]
        //public ActionResult UploadQuestion()
        //{
        //    return View();
        //}


        // GET: AssessmentsController/Edit/5
        public async Task<ActionResult> Edit(int id)
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
            string QuestionFolder  = "QuestionsAndAnswers/Questions";

            var path = Path.Combine(QuestionFolder,fileName+Path.GetExtension(fileName));

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            var contentType = "APPLICATION/octet-stream";
            return File(memory, contentType, Path.GetFileName(path));
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
